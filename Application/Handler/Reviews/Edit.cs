using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Reviews
{
    public class Edit
    {
        public class Command : IRequest<Result<Review>>
        {
            public ReviewInputDto review { get; set; }
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
                var reviewExist = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == request.review.Id);
                if (reviewExist == null)
                {
                    return Result<Review>.Failure("Review not found.");
                }

                _mapper.Map(request.review, reviewExist);

                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Review>.Failure("Faild to edit review");
                return Result<Review>.Success(_context.Entry(reviewExist).Entity);
            }
        }
    }
}
