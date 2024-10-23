using Application.Core;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;


namespace Application.Handler.Reviews
{
    public class Detail
    {
        public class Query : IRequest<Result<Review>>
        {
            public int Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<Review>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<Review>> Handle(Query request, CancellationToken cancellationToken)
            {
                var review = await _context.Reviews
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (review is null)
                    return Result<Review>.Failure("Review not found");
                return Result<Review>.Success(review);
            }
        }
    }
}
