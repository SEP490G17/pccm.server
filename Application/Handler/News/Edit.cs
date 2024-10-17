using Application.Core;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.News
{
    public class Edit
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
        public class Handler : IRequestHandler<Command, Result<NewsBlog>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<NewsBlog>> Handle(Command request, CancellationToken cancellationToken)
            {
                _context.NewsBlogs.Update(request.Event);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<NewsBlog>.Failure("Faild to edit event");
                return Result<NewsBlog>.Success(_context.Entry(request.Event).Entity);
            }
        }
    }
}