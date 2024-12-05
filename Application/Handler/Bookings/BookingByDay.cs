

using Application.Core;
using Application.DTOs;
using Application.Helper;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class BookingByDay
    {
        public class Command : IRequest<Result<BookingDtoV1>>
        {
            public BookingByDayDto Booking { get; set; }
        }


        public class Handler : IRequestHandler<Command, Result<BookingDtoV1>>
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
            public async Task<Result<BookingDtoV1>> Handle(Command request, CancellationToken cancellationToken)
            {
                //1. Lây booking và user từ request
                var bookingRequest = request.Booking;
                var userClaims = _userAccessor.GetUserName();
                var user = await _userManager.FindByNameAsync(userClaims);
                //2. Check đièu kiện
                if (user == null)
                {
                    return Result<BookingDtoV1>.Failure("Username không tồn tại");
                }
                var court = await _context.Courts.Include(c => c.CourtPrices).Include(c => c.CourtCluster).FirstOrDefaultAsync(x => x.Id == request.Booking.CourtId, cancellationToken);

                if (court == null)
                {
                    return Result<BookingDtoV1>.Failure("Sân không tồn tại");
                }

                var courtCluster = court.CourtCluster;
                if (request.Booking.ToTime < request.Booking.FromTime.AddHours(1))
                {
                    return Result<BookingDtoV1>.Failure("Giờ bắt đầu phải lớn hơn giờ kết thúc ít nhất 1 tiếng");
                }
                
                if (request.Booking.ToTime > courtCluster.CloseTime || request.Booking.FromTime < courtCluster.CloseTime)
                {
                    return Result<BookingDtoV1>.Failure($"Thời gian đặt sân không hợp lệ, phải đặt trong thời gian mở/đóng sân");
                }


                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");

                DateTime startDateTimeInLocal = TimeZoneInfo.ConvertTimeFromUtc(bookingRequest.FromDate, vietnamTimeZone);
                //1. startTime chuẩn
                DateTime startDateWithTime = startDateTimeInLocal
                    .Date
                    .AddHours(bookingRequest.FromTime.Hour)
                    .AddMinutes(bookingRequest.FromTime.Minute);
                //2. endTime chuẩn
                DateTime endDateWithTime = startDateTimeInLocal
                    .Date
                    .AddHours(bookingRequest.ToTime.Hour)
                    .AddMinutes(bookingRequest.ToTime.Minute);
                //3. Until chuẩn    

                // Chuyển đổi thời gian từ GMT+7 về UTC
                DateTime startDateTimeUtc = startDateWithTime.ToUniversalTime();
                DateTime endDateTimeUtc = endDateWithTime.ToUniversalTime();

                if (request.Booking.FromDate.Date < DateTime.Today.Date)
                {
                    return Result<BookingDtoV1>.Failure("Không thể đặt lịch ngày trước ngày hiện tại");
                }
                var hasConflictWithSingleDayBookings = await _context.Bookings
                .AnyAsync(x =>
                    x.Court.Id == request.Booking.CourtId &&
                    (int)x.Status == (int)BookingStatus.Confirmed && // Lịch đã xác nhận
                    !x.UntilTime.HasValue &&
                    (
                        (x.StartTime.AddHours(7) <= startDateWithTime && x.EndTime.AddHours(7) > startDateWithTime)
                        ||
                        (x.StartTime.AddHours(7) >= startDateWithTime && x.EndTime.AddHours(7) <= endDateWithTime)
                        ||
                        (x.StartTime.AddHours(7) < endDateWithTime && x.EndTime.AddHours(7) >= endDateWithTime)
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
                           startDateWithTime.Date >= x.StartTime.AddHours(7).Date
                           &&
                           endDateWithTime.Date <= x.UntilTime.Value.AddHours(7).Date
                           &&
                           (
                               (x.StartTime.AddHours(7).TimeOfDay <= startDateWithTime.TimeOfDay && x.EndTime.AddHours(7).TimeOfDay > endDateWithTime.TimeOfDay)
                               ||
                               (x.StartTime.AddHours(7).TimeOfDay >= startDateWithTime.TimeOfDay && x.EndTime.AddHours(7).TimeOfDay <= endDateWithTime.TimeOfDay)
                               ||
                               (x.StartTime.AddHours(7).TimeOfDay < endDateWithTime.TimeOfDay && x.EndTime.AddHours(7).TimeOfDay >= startDateWithTime.TimeOfDay)
                           )
                        )
                    );

                if (hasConflictWithRecurringBookings)
                {
                    return Result<BookingDtoV1>.Failure("Trùng với một số lịch đặt theo combo đã được confirm trước đó");
                }
                // Map sang và bắt đầu tính toán
                var booking = new Booking();
                TimeSpan difference = bookingRequest.ToTime.ToTimeSpan() - bookingRequest.FromTime.ToTimeSpan();
                //duration
                var duration = (int)difference.TotalMinutes;
                //1. amount
                var amount = Utils.CalculateCourtPrice(startDateTimeUtc, endDateTimeUtc, [.. court.CourtPrices]);
                //2. startTime
                booking.StartTime = startDateTimeUtc;
                //3. endTime
                booking.EndTime = endDateTimeUtc;
                //4. until
                // 5. gán court
                booking.Court = court;
                // 6. gán totalPricce
                booking.TotalPrice = amount;
                // 7. kiểm tra roles đểxem có accept luôn không
                booking.Duration = duration;
                booking.PhoneNumber = bookingRequest.PhoneNumber;
                booking.FullName = bookingRequest.FullName;
                booking.AppUser = user;
                var roles = await _userManager.GetRolesAsync(user);
                var acceptableRole = new List<string>() { "Admin", "ManagerCourtCluster", "Owner", "ManagerBooking" };

                // Check nếu là nhân viên có quyền sẽ confirm và tạo thanh toán luôn
                var check = roles.Any(x => acceptableRole.Contains(x));
                if (check)
                {
                    booking.AcceptedAt = DateTime.Now;
                    booking.Status = BookingStatus.Confirmed;
                    var payment = new Payment()
                    {
                        Amount = amount,
                        Status = PaymentStatus.Pending,
                    };
                    booking.Payment = payment;
                    var staffDetail = await _context.StaffDetails.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
                    booking.Staff = staffDetail;
                }
               
                await _context.AddAsync(booking, cancellationToken);

           
                  //  await _context.SaveChangesAsync(cancellationToken);
               
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<BookingDtoV1>.Failure("Fail to create booking");
                var newBooking = _context.Entry(booking).Entity;
                var response = await _context.Bookings
                    .ProjectTo<BookingDtoV1>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == newBooking.Id);
                return Result<BookingDtoV1>.Success(response);
            }



        }

    }
}