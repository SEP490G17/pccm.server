using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class AcceptedBooking
    {
        public class Command : IRequest<Result<BookingDtoV2>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BookingDtoV2>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<BookingDtoV2>> Handle(Command request, CancellationToken cancellationToken)
            {
                var booking = await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Court)
                .ThenInclude(b => b.CourtCluster)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
                
                var today = DateTime.UtcNow.AddHours(7);
                if(booking.StartTime.AddHours(7) < today){
                    return Result<BookingDtoV2>.Failure("Lịch đặt đã quá hạn");
                }

                if (booking == null)
                {
                    return Result<BookingDtoV2>.Failure("Booking không được tìm thấy");
                }
                if (!string.IsNullOrEmpty(booking.RecurrenceRule))
                {
                    var hasConflictWithSingleDayBookings = await _context.Bookings
                  .AnyAsync(x =>
                      x.Court.Id == booking.Court.Id && // Cùng sân
                      (int)x.Status == (int)BookingStatus.Confirmed && // Lịch đã xác nhận
                      !x.UntilTime.HasValue && // Không phải lịch combo
                     (
                      x.StartTime.AddHours(7).Date >= booking.StartTime.AddHours(7).Date &&
                      x.EndTime.AddHours(7).Date <= booking.UntilTime.Value.AddHours(7).Date &&
                      (
                          (booking.StartTime.AddHours(7).TimeOfDay <= x.StartTime.AddHours(7).TimeOfDay && booking.EndTime.AddHours(7).TimeOfDay > x.StartTime.AddHours(7).TimeOfDay)
                          ||
                          (booking.StartTime.AddHours(7).TimeOfDay >= x.StartTime.AddHours(7).TimeOfDay && booking.EndTime.AddHours(7).TimeOfDay <= x.EndTime.AddHours(7).TimeOfDay)
                          ||
                          (booking.StartTime.AddHours(7).TimeOfDay < x.EndTime.AddHours(7).TimeOfDay && booking.EndTime.AddHours(7).TimeOfDay > x.EndTime.AddHours(7).TimeOfDay)

                      )
                     ), cancellationToken
                  );
                    if (hasConflictWithSingleDayBookings)
                    {
                        return Result<BookingDtoV2>.Failure("Trùng với một số lịch đặt theo ngày đã được confirm trước đó");
                    }
                    var hasConflictWithRecurringBookings = await _context.Bookings
                         .AnyAsync(x =>
                             x.Court.Id == booking.Court.Id && // Cùng sân
                             (int)x.Status == (int)BookingStatus.Confirmed && // Lịch đã được xác nhận
                             x.UntilTime.HasValue && // Là lịch combo
                             (
                                 x.StartTime.AddHours(7).Date <= booking.UntilTime.Value.AddHours(7).Date &&
                                 x.UntilTime.HasValue && x.UntilTime.Value.AddHours(7).Date >= booking.StartTime.AddHours(7).Date)
                                 &&
                                 (
                                     (booking.StartTime.AddHours(7).TimeOfDay <= x.StartTime.AddHours(7).TimeOfDay && booking.EndTime.AddHours(7).TimeOfDay > x.StartTime.AddHours(7).TimeOfDay)
                                     ||
                                     (booking.StartTime.AddHours(7).TimeOfDay >= x.StartTime.AddHours(7).TimeOfDay && booking.EndTime.AddHours(7).TimeOfDay <= x.EndTime.AddHours(7).TimeOfDay)
                                     ||
                                     (booking.StartTime.AddHours(7).TimeOfDay < x.EndTime.AddHours(7).TimeOfDay && booking.EndTime.AddHours(7).TimeOfDay > x.EndTime.AddHours(7).TimeOfDay)
                                )
                         );
                    if (hasConflictWithRecurringBookings)
                    {
                        return Result<BookingDtoV2>.Failure("Trùng với một số lịch đặt theo combo đã được confirm trước đó");
                    }
                }
                else
                {
                    var hasConflictWithSingleDayBookings = await _context.Bookings
                                   .AnyAsync(x =>
                                       x.Court.Id == booking.Court.Id &&
                                       (int)x.Status == (int)BookingStatus.Confirmed && // Lịch đã xác nhận
                                       !x.UntilTime.HasValue &&
                                       (
                                           (x.StartTime.AddHours(7) <= booking.StartTime.AddHours(7) && x.EndTime.AddHours(7) > booking.StartTime.AddHours(7))
                                           ||
                                           (x.StartTime.AddHours(7) >= booking.StartTime.AddHours(7) && x.EndTime.AddHours(7) <= booking.EndTime.AddHours(7))
                                           ||
                                           (x.StartTime.AddHours(7) < booking.EndTime.AddHours(7) && x.EndTime.AddHours(7) >= booking.EndTime.AddHours(7))
                                       ), cancellationToken
                                   );
                    if (hasConflictWithSingleDayBookings)
                    {
                        return Result<BookingDtoV2>.Failure("Trùng với một số lịch đặt theo ngày đã được confirm trước đó");
                    }

                    var hasConflictWithRecurringBookings = await _context.Bookings
                        .AnyAsync(x =>
                            x.Court.Id == booking.Court.Id && // Cùng sân
                            (int)x.Status == (int)BookingStatus.Confirmed && // Lịch đã được xác nhận
                            x.UntilTime.HasValue && // Là lịch combo
                            (
                               booking.StartTime.AddHours(7).Date >= x.StartTime.AddHours(7).Date
                               &&
                               booking.EndTime.AddHours(7).Date <= x.UntilTime.Value.AddHours(7).Date
                               &&
                               (
                                   (x.StartTime.AddHours(7).TimeOfDay <= booking.StartTime.AddHours(7).TimeOfDay && x.EndTime.AddHours(7).TimeOfDay > booking.EndTime.AddHours(7).TimeOfDay)
                                   ||
                                   (x.StartTime.AddHours(7).TimeOfDay >= booking.StartTime.AddHours(7).TimeOfDay && x.EndTime.AddHours(7).TimeOfDay <= booking.EndTime.AddHours(7).TimeOfDay)
                                   ||
                                   (x.StartTime.AddHours(7).TimeOfDay < booking.EndTime.AddHours(7).TimeOfDay && x.EndTime.AddHours(7).TimeOfDay >= booking.StartTime.AddHours(7).TimeOfDay)
                               )
                            )
                        );

                    if (hasConflictWithRecurringBookings)
                    {
                        return Result<BookingDtoV2>.Failure("Trùng với một số lịch đặt theo combo đã được confirm trước đó");
                    }
                }

                booking.Status = BookingStatus.Confirmed;
                var payment = new Payment()
                {
                    Amount = booking.TotalPrice,
                    Status = PaymentStatus.Pending,
                };
                booking.Payment = payment;
                _context.Bookings.Update(booking);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<BookingDtoV2>.Failure("Accept booking failed.");
                var bookingResponse = _mapper.Map<BookingDtoV2>(_context.Entry(booking).Entity);
                return Result<BookingDtoV2>.Success(bookingResponse);
            }
        }
    }
}