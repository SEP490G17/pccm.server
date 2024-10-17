using Application.Core;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Courts
{
    public class Create
    {
        public class Command : IRequest<Result<Court>>
        {
            public Court Court { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Court).SetValidator(new CourtValidator());
            }
        }

        public class Handler(DataContext _context) : IRequestHandler<Command, Result<Court>>
        {
            public async Task<Result<Court>> Handle(Command request, CancellationToken cancellationToken)
            {
                var court = request.Court;
                await _context.AddAsync(court, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Court>.Failure("Fail to create court");
                var newCourt = _context.Entry(court).Entity;
                return Result<Court>.Success(newCourt);
            }
        }
    }
}