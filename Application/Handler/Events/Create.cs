using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Events
{
    public class Create
    {
        public class Command : IRequest<Result<Event>>
        {
            public Event Event { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Event).SetValidator(new EventValidator());
            }
        }

        public class Handler(DataContext _context) : IRequestHandler<Command, Result<Event>>
        {
            public async Task<Result<Event>> Handle(Command request, CancellationToken cancellationToken)
            {
                var even = request.Event;
                await _context.AddAsync(even, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Event>.Failure("Fail to create event");
                var newEvent = _context.Entry(even).Entity;
                return Result<Event>.Success(newEvent);
            }
        }
    }
}