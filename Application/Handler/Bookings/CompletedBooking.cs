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
        public class Command : IRequest<Result<BookingDtoV1>>
        {
            public int Id { get; set; }
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


                var booking = await _context.Bookings.Include(b=>b.Payment).FirstOrDefaultAsync(x => x.Id == request.Id);
                if (booking == null)
                {
                    return Result<BookingDtoV1>.Failure("Booking không được tìm thấy");
                }
                if((int)booking.Payment.Status != (int)PaymentStatus.Success){
                    return Result<BookingDtoV1>.Failure("Booking chưa được thanh toán");

                }
                booking.IsSuccess = true;

                _context.Bookings.Update(booking);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<BookingDtoV1>.Failure("Updated failed booking.");
                return Result<BookingDtoV1>.Success(_mapper.Map<BookingDtoV1>(_context.Entry(booking).Entity));
            }
        }
    }
}