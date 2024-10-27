using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
                var even = _mapper.Map<NewsBlog>(request.Event);
                await _context.AddAsync(even, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<NewsBlog>.Failure("Fail to create event");
                var newEvent = _context.Entry(even).Entity;
                return Result<NewsBlog>.Success(newEvent);
            }
        }
    }
}