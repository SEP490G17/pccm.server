using Application.Core;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.News
{
    public class Create
    {
        public class Command : IRequest<Result<NewsBlog>>
        {
            public NewsBlog Event { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Event).SetValidator(new EventValidator());
            }
        }

        public class Handler(DataContext _context) : IRequestHandler<Command, Result<NewsBlog>>
        {
            public async Task<Result<NewsBlog>> Handle(Command request, CancellationToken cancellationToken)
            {
                var even = request.Event;
                await _context.AddAsync(even, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<NewsBlog>.Failure("Fail to create event");
                var newEvent = _context.Entry(even).Entity;
                return Result<NewsBlog>.Success(newEvent);
            }
        }
    }
}