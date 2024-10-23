using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Reviews
{
    public class Edit
    {
        public class Command : IRequest<Result<Review>>
        {
            public ReviewInputDTO review { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.review).SetValidator(new ReviewValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<Review>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Review>> Handle(Command request, CancellationToken cancellationToken)
            {
                var review = _mapper.Map<Review>(request.review);
                var reviewExist = await _context.Reviews.FindAsync(request.review.Id);
                _mapper.Map(review, reviewExist);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Review>.Failure("Faild to edit review");
                return Result<Review>.Success(_context.Entry(review).Entity);
            }
        }
    }
}
