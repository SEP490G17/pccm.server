

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
                //1. Lây booking và user từ request
                var bookingRequest = request.Booking;
                var userClaims = _userAccessor.GetUserName();
                // var user = await _userManager.FindByNameAsync(userClaims);
                //2. Check đièu kiện
                // if (user == null)
                // {
                //     return Result<List<BookingDtoV2>>.Failure("Username không tồn tại");
                // }
                var court = await _context.Courts.FirstOrDefaultAsync(x => x.Id == request.Booking.CourtId, cancellationToken);
                if (court == null)
                {
                    return Result<List<BookingDtoV2>>.Failure("Sân không tồn tại");
                }

                DateTime startDateTimeInLocal = bookingRequest.FromDate.ToLocalTime();
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

                // Check trùng, bao phủ, va chạm lịch
                var conflictingBookings = await _context.Bookings
                .Where(x =>
                    x.Id != request.Booking.BookingId &&
                    x.Court.Id == request.Booking.CourtId && // Same court
                    (int)x.Status == (int)BookingStatus.Pending && // Confirmed bookings
                    (
                        // Check conflict with single booking
                        (!x.UntilTime.HasValue && x.StartTime.Date == startDateTimeUtc.Date) // Same day
                        ||
                        // Check conflict with recurring bookings
                        (x.UntilTime.HasValue && x.StartTime.Date <= startDateTimeUtc.Date && x.UntilTime.Value.Date >= startDateTimeUtc.Date) // Within date range
                    )
                    &&
                    (
                        // Check exact time conflict
                        (startDateTimeUtc == x.StartTime && endDateTimeUtc == x.EndTime)
                        ||
                        // Check overlapping time
                        (startDateTimeUtc < x.EndTime && endDateTimeUtc > x.StartTime)
                        ||
                        // Check encompassing time
                        (startDateTimeUtc <= x.StartTime && endDateTimeUtc >= x.EndTime)
                    )
                )
                .ProjectTo<BookingDtoV2>(_mapper.ConfigurationProvider)
                .ToListAsync();
                return Result<List<BookingDtoV2>>.Success(conflictingBookings);
            }


        }
    }
}