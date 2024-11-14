using Application.Core;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Categories
{
    public class Edit
    {
        public class Command : IRequest<Result<Category>>
        {
            public Category Category { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Category).SetValidator(new CategoryValidator());

            }
        }
        public class Handler : IRequestHandler<Command, Result<Category>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Category>> Handle(Command request, CancellationToken cancellationToken)
            {
                _context.Categories.Update(request.Category);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Category>.Failure("Faild to edit category");
                return Result<Category>.Success(_context.Entry(request.Category).Entity);
            }
        }
    }
}
