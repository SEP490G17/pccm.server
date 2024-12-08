

using Application.Core;
using Application.DTOs;
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
    public class BookingWithCombo
    {
        public class Command : IRequest<Result<BookingDtoV1>>
        {
            public BookingWithComboDto Booking { get; set; }
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


                var court = await _context.Courts.Include(c => c.CourtCluster).FirstOrDefaultAsync(x => x.Id == request.Booking.CourtId, cancellationToken);

                if (court == null)
                {
                    return Result<BookingDtoV1>.Failure("Sân không tồn tại");
                }
               
                var courtCluster = court.CourtCluster;

                if (request.Booking.ToTime < request.Booking.FromTime.AddHours(1))
                {
                    return Result<BookingDtoV1>.Failure("Giờ bắt đầu phải lớn hơn giờ kết thúc ít nhất 1 tiếng");
                }
                
                if (request.Booking.ToTime > courtCluster.CloseTime || request.Booking.FromTime < courtCluster.OpenTime)
                {
                    return Result<BookingDtoV1>.Failure($"Thời gian đặt sân không hợp lệ, phải đặt trong thời gian mở/đóng sân");
                }
                //3. Lấy combo để có endtime chuẩn
                var combo = await _context.CourtCombos.FirstOrDefaultAsync(cc => cc.Id == request.Booking.ComboId, cancellationToken);
                if (combo == null)
                {
                    return Result<BookingDtoV1>.Failure("Combo không tồn tại");

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

                var until = endDateTimeUtc.AddMonths(combo.Duration);
                if (request.Booking.FromDate.Date < DateTime.Today.Date)
                {
                    return Result<BookingDtoV1>.Failure("Không thể đặt lịch ngày trước ngày hiện tại");
                }
                // Check trùng, bao phủ, va chạm lịch
                var hasConflictWithSingleDayBookings = await _context.Bookings
                    .AnyAsync(x =>
                        x.Court.Id == request.Booking.CourtId && // Cùng sân
                        (int)x.Status == (int)BookingStatus.Confirmed && // Lịch đã xác nhận
                        !x.UntilTime.HasValue && // Không phải lịch combo
                       (
                        x.StartTime.AddHours(7).Date >= startDateWithTime.Date &&
                        x.EndTime.AddHours(7).Date <= until.AddHours(7).Date &&
                        (
                            (startDateWithTime.TimeOfDay <= x.StartTime.AddHours(7).TimeOfDay && endDateWithTime.TimeOfDay > x.StartTime.AddHours(7).TimeOfDay)
                            ||
                            (startDateWithTime.TimeOfDay >= x.StartTime.AddHours(7).TimeOfDay && endDateWithTime.TimeOfDay <= x.EndTime.AddHours(7).TimeOfDay)
                            ||
                            (startDateWithTime.TimeOfDay < x.EndTime.AddHours(7).TimeOfDay && endDateWithTime.TimeOfDay > x.EndTime.AddHours(7).TimeOfDay)

                        )
                       ), cancellationToken
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
                             x.StartTime.AddHours(7).Date <= until.AddHours(7).Date &&
                             x.UntilTime.HasValue && x.UntilTime.Value.AddHours(7).Date >= startDateTimeUtc.AddHours(7).Date)
                             &&
                             (
                                 (startDateWithTime.TimeOfDay <= x.StartTime.AddHours(7).TimeOfDay && endDateWithTime.TimeOfDay > x.StartTime.AddHours(7).TimeOfDay)
                                 ||
                                 (startDateWithTime.TimeOfDay >= x.StartTime.AddHours(7).TimeOfDay && endDateWithTime.TimeOfDay <= x.EndTime.AddHours(7).TimeOfDay)
                                 ||
                                 (startDateWithTime.TimeOfDay < x.EndTime.AddHours(7).TimeOfDay && endDateWithTime.TimeOfDay > x.EndTime.AddHours(7).TimeOfDay)
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
                var amout = duration / 60 * combo.TotalPrice;
                //2. startTime
                booking.StartTime = startDateTimeUtc;
                //3. endTime
                booking.EndTime = endDateTimeUtc;
                //4. until
                booking.UntilTime = until;
                // 5. gán court
                booking.Court = court;
                // 6. gán totalPricce
                booking.TotalPrice = amout;
                // 7. kiểm tra roles đểxem có accept luôn không
                booking.Duration = duration;
                booking.PhoneNumber = bookingRequest.PhoneNumber;
                booking.FullName = bookingRequest.FullName;
                var recurrenceRule = new RecurrenceRule()
                {
                    Frequency = "DAILY",
                    Interval = 1,
                    Until = until,
                };
                booking.RecurrenceRule = recurrenceRule.ToString();
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
                        Amount = amout,
                        Status = PaymentStatus.Pending,
                    };
                    booking.Payment = payment;
                    var staffDetail = await _context.StaffDetails.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
                    booking.Staff = staffDetail;
                }
               // booking.AppUser = user;
              
                await _context.AddAsync(booking, cancellationToken);
                
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