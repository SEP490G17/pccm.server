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

                var checkSlot = await _context.Bookings
                        .AnyAsync(x =>
                            x.Court.Id == request.Booking.CourtId && // Cùng sân
                            (int)x.Status == (int)BookingStatus.Confirmed && // Lịch đã được xác nhận

                            // Kiểm tra va chạm trùng hay gì đó, không nghĩ ra từ với lịch lẻ khác
                            (
                                (!x.UntilTime.HasValue && // Lịch lẻ
                                x.StartTime.Date >= request.Booking.StartTime.Date &&
                                x.StartTime.Date <= request.Booking.EndTime.Date)
                                ||
                                // Kiểm tra va chạm với lịch dài hạn
                                (x.UntilTime.HasValue && // Lịch dài hạn
                                x.StartTime.Date <= request.Booking.EndTime.Date &&
                                x.UntilTime.Value.Date >= request.Booking.StartTime.Date)
                            )

                            // Kiểm tra va chạm thời gian
                            && request.Booking.StartTime.TimeOfDay < x.EndTime.TimeOfDay
                            && request.Booking.EndTime.TimeOfDay > x.StartTime.TimeOfDay
                        );
                if (checkSlot)
                {
                    return Result<BookingDtoV1>.Failure("Trùng lịch của 1 booking đã được confirm trước đó");
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