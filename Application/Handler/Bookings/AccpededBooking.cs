using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using MediatR;
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
                if (booking == null)
                {
                    return Result<BookingDtoV2>.Failure("Booking không được tìm thấy");
                }
                var checkSlot = await _context.Bookings.AnyAsync(x =>
                    x.Court.Id != booking.Court.Id &&
                    (int)x.Status == (int)BookingStatus.Confirmed
                    && ((booking.StartTime <= x.StartTime && booking.EndTime > x.StartTime)
                    || (booking.StartTime < x.EndTime && booking.EndTime > x.EndTime)
                    || (booking.StartTime >= x.StartTime && booking.EndTime <= x.EndTime))
                );
                if (checkSlot)
                {
                    return Result<BookingDtoV2>.Failure("Trùng lịch của 1 booking đã được confirm trước đó");
                }
                booking.Status = BookingStatus.Confirmed;
                var payment = new Payment()
                {
                    Amount = booking.TotalPrice,
                    Status = PaymentStatus.Pending,
                };
                booking.Payment = payment;
                _context.Bookings.Update(booking);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<BookingDtoV2>.Failure("Accept booking failed.");
                return Result<BookingDtoV2>.Success(_mapper.Map<BookingDtoV2>(_context.Entry(booking).Entity));
            }
        }
    }
}