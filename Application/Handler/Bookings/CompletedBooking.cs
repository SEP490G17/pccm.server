using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class CompletedBooking
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
                .Include(b=>b.Payment)
                .Include(b=>b.Court)
                .ThenInclude(b=>b.CourtCluster)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (booking == null)
                {
                    return Result<BookingDtoV2>.Failure("Booking không được tìm thấy");
                }
                if((int)booking.Payment.Status != (int)PaymentStatus.Success){
                    return Result<BookingDtoV2>.Failure("Booking chưa được thanh toán");
                }
                var pendingOrder = _context.Orders.Any(o=>o.BookingId == booking.Id && (int)o.Payment.Status == (int)PaymentStatus.Pending);
                if (pendingOrder){
                    return Result<BookingDtoV2>.Failure("Còn đơn Order chưa được thanh toán");
                }
                booking.IsSuccess = true;

                _context.Bookings.Update(booking);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<BookingDtoV2>.Failure("Updated failed booking.");
                return Result<BookingDtoV2>.Success(_mapper.Map<BookingDtoV2>(_context.Entry(booking).Entity));
            }
        }
    }
}