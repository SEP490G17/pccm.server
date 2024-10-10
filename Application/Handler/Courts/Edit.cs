using Application.Core;
using AutoMapper;
using Domain;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Courts
{
    public class Edit
    {
        public class Command : IRequest<Result<Court>>
        {
            public Court court { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.court).SetValidator(new CourtValidator());

            }
        }
        public class Handler : IRequestHandler<Command, Result<Court>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Court>> Handle(Command request, CancellationToken cancellationToken)
            {
                _context.Courts.Update(request.court);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Court>.Failure("Faild to edit court");
                return Result<Court>.Success(_context.Entry(request.court).Entity);
            }
        }
    }
}