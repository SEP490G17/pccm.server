using Application.Core;
using AutoMapper;
using Domain;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Events
{
    public class Edit
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
        public class Handler : IRequestHandler<Command, Result<Event>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Event>> Handle(Command request, CancellationToken cancellationToken)
            {
                _context.Events.Update(request.Event);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Event>.Failure("Faild to edit event");
                return Result<Event>.Success(_context.Entry(request.Event).Entity);
            }
        }
    }
}