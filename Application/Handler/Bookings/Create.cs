using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Bookings
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public BookingInputDto Booking { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Booking).SetValidator(new BookingValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var booking = _mapper.Map<Booking>(request.Booking);
                booking.AcceptedAt = DateTime.Now;
              
                await _context.AddAsync(booking, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Unit>.Failure("Fail to create booking");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}