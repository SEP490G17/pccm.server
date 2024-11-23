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
    public class Create
    {
        public class Command : IRequest<Result<ReviewDto>>
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
        public class Handler : IRequestHandler<Command, Result<ReviewDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<ReviewDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var review = _mapper.Map<Review>(request.review);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.review.phoneNumber, cancellationToken);
                review.UserId = user.Id;
                review.User = user;
                review.CourtCluster = await _context.CourtClusters.FirstOrDefaultAsync(c => c.Id == request.review.CourtClusterId, cancellationToken);
                await _context.AddAsync(review, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<ReviewDto>.Failure("Fail to create review");
                var newReview = _context.Entry(review).Entity;
                var res = _mapper.Map<ReviewDto>(newReview);
                return Result<ReviewDto>.Success(res);
            }
        }
    }
}
