using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class StaffCreate
    {
        public class Command : IRequest<Result<BookingDtoV1>>
        {
            public BookingInputDto Booking { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Booking).SetValidator(new BookingValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<BookingDtoV1>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<BookingDtoV1>> Handle(Command request, CancellationToken cancellationToken)
            {
                var today = DateTime.UtcNow.AddHours(7);

                if (request.Booking.StartTime.Date < DateTime.Today.Date)
                {
                    return Result<BookingDtoV1>.Failure("Không thể đặt lịch ngày trước ngày hiện tại");
                }
                var bookingRequest = request.Booking;
                var hasConflictWithSingleDayBookings = await _context.Bookings
                .AnyAsync(x =>
                    x.Court.Id == request.Booking.CourtId &&
                    (int)x.Status == (int)BookingStatus.Confirmed && // Lịch đã xác nhận
                    !x.UntilTime.HasValue &&
                    (
                        (x.StartTime.AddHours(7) <= bookingRequest.StartTime.AddHours(7) && x.EndTime.AddHours(7) > bookingRequest.StartTime.AddHours(7))
                        ||
                        (x.StartTime.AddHours(7) >= bookingRequest.StartTime.AddHours(7) && x.EndTime.AddHours(7) <= bookingRequest.EndTime.AddHours(7))
                        ||
                        (x.StartTime.AddHours(7) < bookingRequest.EndTime.AddHours(7) && x.EndTime.AddHours(7) >= bookingRequest.EndTime.AddHours(7))
                    )
                );
                if (hasConflictWithSingleDayBookings)
                {
                    return Result<BookingDtoV1>.Failure("Trùng với một số lịch đặt theo ngày đã được confirm trước đó");
                }

                var hasConflictWithRecurringBookings = await _context.Bookings
                    .AnyAsync(x =>
                        x.Court.Id == request.Booking.CourtId && // Cùng sân
                        (int)x.Status == (int)BookingStatus.Confirmed && // Lịch đã được xác nhận
                        x.UntilTime.HasValue && // Là lịch combo
                        (
                           bookingRequest.StartTime.AddHours(7).Date >= x.StartTime.AddHours(7).Date
                           &&
                           bookingRequest.EndTime.AddHours(7).Date <= x.UntilTime.Value.AddHours(7).Date
                           &&
                           (
                               (x.StartTime.AddHours(7).TimeOfDay <= bookingRequest.StartTime.AddHours(7).TimeOfDay && x.EndTime.AddHours(7).TimeOfDay > bookingRequest.EndTime.AddHours(7).TimeOfDay)
                               ||
                               (x.StartTime.AddHours(7).TimeOfDay >= bookingRequest.StartTime.AddHours(7).TimeOfDay && x.EndTime.AddHours(7).TimeOfDay <= bookingRequest.EndTime.AddHours(7).TimeOfDay)
                               ||
                               (x.StartTime.AddHours(7).TimeOfDay < bookingRequest.EndTime.AddHours(7).TimeOfDay && x.EndTime.AddHours(7).TimeOfDay >= bookingRequest.StartTime.AddHours(7).TimeOfDay)
                           )
                        )
                    );

                if (hasConflictWithRecurringBookings)
                {
                    return Result<BookingDtoV1>.Failure("Trùng với một số lịch đặt theo combo đã được confirm trước đó");
                }

                var todayDate = DateTime.UtcNow;

                if (request.Booking.StartTime < todayDate)
                {
                    return Result<BookingDtoV1>.Failure("Không thể đặt lịch ngày trước ngày hiện tại");
                }

                var court = await _context.Courts.Include(x => x.CourtPrices).FirstOrDefaultAsync(x => x.Id == request.Booking.CourtId);

                if (court == null || court.Status == CourtStatus.Closed)
                {
                    return Result<BookingDtoV1>.Failure("Sân không tồn tại hoặc đã dừng hoạt động");
                }
                var courtPrice = court.CourtPrices.ToList();
                var amout = CalculateCourtPrice(request.Booking.StartTime, request.Booking.EndTime, courtPrice);

                var booking = _mapper.Map<Booking>(request.Booking);
                booking.Status = BookingStatus.Confirmed;
                booking.TotalPrice = amout;
                booking.Court = court;
                booking.Duration = (int)booking.EndTime.Subtract(booking.StartTime).TotalMinutes;
                var payment = new Payment()
                {
                    Amount = amout,
                    Status = PaymentStatus.Pending,
                };
                booking.Payment = payment;
                await _context.AddAsync(booking, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<BookingDtoV1>.Failure("Fail to create booking");
                return Result<BookingDtoV1>.Success(_mapper.Map<BookingDtoV1>(booking));


            }

            public static decimal CalculateCourtPrice(DateTime fromTime, DateTime toTime, List<CourtPrice> courtPrices)
            {
                decimal totalPrice = 0;

                TimeZoneInfo gmtPlus7 = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");

                // Chuyển đổi từ UTC hoặc giờ hệ thống sang giờ GMT+7
                DateTime fromTimeGmt7 = TimeZoneInfo.ConvertTime(fromTime, gmtPlus7);
                DateTime toTimeGmt7 = TimeZoneInfo.ConvertTime(toTime, gmtPlus7);

                // Chuyển từ DateTime sang TimeOnly để so sánh
                TimeOnly startTimeOnly = TimeOnly.FromDateTime(fromTimeGmt7);
                TimeOnly endTimeOnly = TimeOnly.FromDateTime(toTimeGmt7);
                // Sắp xếp các mức giá theo thời gian
                courtPrices = courtPrices.OrderBy(cp => cp.FromTime).ToList();

                while (startTimeOnly < endTimeOnly)
                {
                    // Tìm mức giá phù hợp với thời gian bắt đầu
                    var currentPrice = courtPrices.FirstOrDefault(cp => startTimeOnly >= cp.FromTime && startTimeOnly < cp.ToTime);

                    if (currentPrice != null)
                    {
                        // Tính thời gian thuê trong khoảng giá hiện tại
                        TimeOnly nextPriceChange = endTimeOnly < currentPrice.ToTime ? endTimeOnly : currentPrice.ToTime;
                        decimal hours = (decimal)(nextPriceChange.ToTimeSpan() - startTimeOnly.ToTimeSpan()).TotalHours;
                        totalPrice += hours * currentPrice.Price;

                        // Cập nhật thời gian bắt đầu để tính cho mức giá tiếp theo
                        startTimeOnly = nextPriceChange;
                    }
                    else
                    {
                        // Nếu không có mức giá nào phù hợp, thoát vòng lặp
                        break;
                    }
                }

                return totalPrice;
            }

        }
    }
}