

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
    public class GetBookingConflict
    {
        public class Command : IRequest<Result<List<BookingDtoV2>>>
        {
            public BookingConflictDto Booking { get; set; }
        }


        public class Handler : IRequestHandler<Command, Result<List<BookingDtoV2>>>
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
            public async Task<Result<List<BookingDtoV2>>> Handle(Command request, CancellationToken cancellationToken)
            {
                var bookingRequest = request.Booking;
                var userClaims = _userAccessor.GetUserName();
                var user = await _userManager.FindByNameAsync(userClaims);
                //2. Check đièu kiện
                if (user == null)
                {
                    return Result<List<BookingDtoV2>>.Failure("Username không tồn tại");
                }
                var court = await _context.Courts.Include(c => c.CourtPrices).FirstOrDefaultAsync(x => x.Id == request.Booking.CourtId, cancellationToken);
                if (court == null)
                {
                    return Result<List<BookingDtoV2>>.Failure("Sân không tồn tại");
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
                var bookingAccept = await _context.Bookings.FirstAsync(b => b.Id == request.Booking.BookingId, cancellationToken);
                if (bookingAccept == null)
                {
                    return Result<List<BookingDtoV2>>.Failure("Booking không tồn tại");
                }
                List<BookingDtoV2> bookingConflict = new List<BookingDtoV2>();

                var hasConflictWithSingleDayBookings = await _context.Bookings
                                .Where(x =>
                                    x.Court.Id == request.Booking.CourtId &&
                                    x.Id != request.Booking.BookingId && //lấy ra các booking khác
                                    (int)x.Status == (int)BookingStatus.Pending && // Lịch đang chờ xác nhận
                                    !x.UntilTime.HasValue &&
                                    (
                                        (x.StartTime.AddHours(7) <= startDateWithTime && x.EndTime.AddHours(7) >= endDateWithTime) // ( [ ] ) [] booking accept () booking check
                                        ||
                                        (x.StartTime.AddHours(7) >= startDateWithTime && x.EndTime.AddHours(7) <= endDateWithTime) //  [ ( ) ] 
                                        ||
                                        (bookingAccept.UntilTime.HasValue
                                        && x.StartTime.AddHours(7) < bookingAccept.UntilTime.Value.AddHours(7)
                                        && x.EndTime.AddHours(7) > bookingAccept.UntilTime.Value.AddHours(7)) // check trường hợp nếu đơn accept là đơn combo thì phải check đến until time cho các đơn ngày. Chứ ko để check endtime được vì endtime chỉ trong ngày nên những lịch ngày hôm sau sẽ không check trùng
                                        ||
                                        (bookingAccept.UntilTime.HasValue
                                        && x.StartTime.AddHours(7) < bookingAccept.UntilTime.Value.AddHours(7)
                                        && x.EndTime.AddHours(7) < bookingAccept.UntilTime.Value.AddHours(7)) // check trường hợp nếu đơn accept là đơn combo thì phải check đến until time cho các đơn ngày. Chứ ko để check endtime được vì endtime chỉ trong ngày nên những lịch ngày hôm sau sẽ không check trùng
                                        ||
                                        (x.StartTime.AddHours(7) < endDateWithTime && x.StartTime.AddHours(7) > startDateWithTime) // [ ( ] )  
                                        ||
                                        (x.EndTime.AddHours(7) < endDateWithTime && x.EndTime.AddHours(7) > startDateWithTime) // ( [ ) ]
                                    )
                                )
                                .ProjectTo<BookingDtoV2>(_mapper.ConfigurationProvider)
                                .ToListAsync(cancellationToken);
                bookingConflict.AddRange(hasConflictWithSingleDayBookings);
                var conflictWithRecurringBookings = await _context.Bookings
                    .Where(x =>
                        x.Court.Id == request.Booking.CourtId && // Cùng sân
                        x.Id != request.Booking.BookingId && // lấy ra các booking khác
                        (int)x.Status == (int)BookingStatus.Pending && // Lịch đang chờ xác nhận
                        x.UntilTime.HasValue && // Là lịch combo
                        (
                                bookingAccept.UntilTime.HasValue &&
                                (
                                    (startDateWithTime.Date >= x.StartTime.AddHours(7).Date
                                    && bookingAccept.UntilTime.Value.AddHours(7) <= x.UntilTime.Value.AddHours(7).Date) // ( [  ] ) [] booking accept () booking check
                                    ||
                                    (startDateWithTime.Date <= x.StartTime.AddHours(7).Date
                                    && bookingAccept.UntilTime.Value.AddHours(7) >= x.UntilTime.Value.AddHours(7).Date) // [ (  ) ]
                                    ||
                                    (startDateWithTime.Date < x.UntilTime.Value.AddHours(7).Date
                                    && bookingAccept.UntilTime.Value.AddHours(7) > x.UntilTime.Value.AddHours(7).Date) // ( [ ) ]
                                    ||
                                    (startDateWithTime.Date < x.StartTime.AddHours(7).Date
                                    && bookingAccept.UntilTime.Value.AddHours(7) > x.StartTime.AddHours(7).Date) // [ ( ] ) 
                                )
                                ||
                                !bookingAccept.UntilTime.HasValue &&
                                (
                                    (startDateWithTime.Date >= x.StartTime.AddHours(7).Date
                                    && endDateWithTime.Date <= x.UntilTime.Value.AddHours(7).Date) // ( [  ] ) [] booking accept () booking check
                                    ||
                                    (startDateWithTime.Date <= x.StartTime.AddHours(7).Date
                                    && endDateWithTime.Date >= x.UntilTime.Value.AddHours(7).Date) // [ (  ) ]
                                    ||
                                    (startDateWithTime.Date < x.UntilTime.Value.AddHours(7).Date
                                    && endDateWithTime.Date > x.UntilTime.Value.AddHours(7).Date) // ( [ ) ]
                                    ||
                                    (startDateWithTime.Date < x.StartTime.AddHours(7).Date
                                    && endDateWithTime.Date > x.StartTime.AddHours(7).Date) // [ ( ] )
                                )

                        )
                        &&
                        (
                            (x.StartTime.AddHours(7).TimeOfDay <= startDateWithTime.TimeOfDay && x.EndTime.AddHours(7).TimeOfDay > endDateWithTime.TimeOfDay)
                            ||
                            (x.StartTime.AddHours(7).TimeOfDay >= startDateWithTime.TimeOfDay && x.EndTime.AddHours(7).TimeOfDay <= endDateWithTime.TimeOfDay)
                            ||
                            (x.StartTime.AddHours(7).TimeOfDay < endDateWithTime.TimeOfDay && x.EndTime.AddHours(7).TimeOfDay >= startDateWithTime.TimeOfDay)
                        )
                    )
                    .ProjectTo<BookingDtoV2>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                // Check trùng, bao phủ, va chạm lịch
                bookingConflict.AddRange(conflictWithRecurringBookings);
                return Result<List<BookingDtoV2>>.Success(bookingConflict);
            }


        }
    }
}