using Application.Core;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class Edit
    {
        public class Command : IRequest<Result<Booking>>
        {
            public int Id { get; set; }
            public string Status { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {

        }
        public class Handler : IRequestHandler<Command, Result<Booking>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Booking>> Handle(Command request, CancellationToken cancellationToken)
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (booking == null)
                {
                    return Result<Booking>.Failure("Booking not found");
                }

                if (request.Status == "accept")
                {
                    booking.Status = Domain.Enum.BookingStatus.Confirmed;
                }
                else if (request.Status == "decline")
                {

                    booking.Status = Domain.Enum.BookingStatus.Declined;
                }
                else
                {
                    return Result<Booking>.Failure("Status booking not valid.");
                }

                _context.Bookings.Update(booking);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Booking>.Failure("Updated failed booking.");
                return Result<Booking>.Success(_context.Entry(booking).Entity);
            }
        }
    }
}