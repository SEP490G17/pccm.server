using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
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

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly UserManager<AppUser> _userManager;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor, UserManager<AppUser> userManager)
            {
                _mapper = mapper;
                _context = context;
                _userAccessor = userAccessor;
                _userManager = userManager;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {

                var userClaims = _userAccessor.GetUserName();
                var user = await _userManager.FindByNameAsync(userClaims);
                if (user == null)
                {
                    return Result<Unit>.Failure("Username không tồn tại");
                }
                var checkSlot = await _context.Bookings.AnyAsync(
                  x =>
                  x.Court.Id != request.Booking.CourtId &&
                      (int)x.Status == (int)BookingStatus.Confirmed
                      && ((request.Booking.StartTime <= x.StartTime && request.Booking.EndTime > x.StartTime)
                      || (request.Booking.StartTime < x.EndTime && request.Booking.EndTime > x.EndTime)
                      || (request.Booking.StartTime >= x.StartTime && request.Booking.EndTime <= x.EndTime))
                  );

                if (checkSlot)
                {
                    return Result<Unit>.Failure("Trùng lịch của 1 booking đã được confirm trước đó");
                }

                if (request.Booking.StartTime.Date < DateTime.Today.Date)
                {
                    return Result<Unit>.Failure("Không thể đặt lịch ngày trước ngày hiện tại");
                }

                var booking = _mapper.Map<Booking>(request.Booking);
                var court = await _context.Courts.Include(x => x.CourtPrices).FirstOrDefaultAsync(x => x.Id == request.Booking.CourtId);
                var courtPrice = court.CourtPrices.ToList();
                var amout = CalculateCourtPrice(request.Booking.StartTime, request.Booking.EndTime, courtPrice);
                booking.Court = await _context.Courts.FirstOrDefaultAsync(c => c.Id == request.Booking.CourtId);
                booking.AcceptedAt = DateTime.Now;
                booking.TotalPrice = amout;
                
                await _context.AddAsync(booking, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Unit>.Failure("Fail to create booking");
                return Result<Unit>.Success(Unit.Value);
            }

            public decimal CalculateCourtPrice(DateTime fromTime, DateTime toTime, List<CourtPrice> courtPrices)
            {
                decimal totalPrice = 0;

                TimeZoneInfo gmtPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

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