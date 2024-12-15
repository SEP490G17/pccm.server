using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class BookingPaymentSuccess
    {
        public class Command : IRequest<Result<BookingDtoV2>>
        {
            public int Id { get; set; }
            public bool IncludeOrder {get;set;}
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
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (booking == null)
                {
                    return Result<BookingDtoV2>.Failure("Booking không được tìm thấy");
                }
                if(request.IncludeOrder){
                    var order = await _context.Orders.Include(o=>o.Payment).Where(o=>o.BookingId == booking.Id).ToListAsync(cancellationToken);
                    if(order.Count > 0){
                        order.ForEach(o=>o.Payment.Status = PaymentStatus.Success);
                        _context.UpdateRange(order);
                    }
                }
                booking.Payment.Status = PaymentStatus.Success;
                _context.Bookings.Update(booking);
                 await _context.SaveChangesAsync(cancellationToken);
                //if (!result) return Result<BookingDtoV2>.Failure("Updated failed booking.");
                return Result<BookingDtoV2>.Success(_mapper.Map<BookingDtoV2>(_context.Entry(booking).Entity));
            }
        }
    }
}