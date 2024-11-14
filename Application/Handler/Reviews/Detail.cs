using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;


namespace Application.Handler.Reviews
{
    public class ListCourtCluster
    {
        public class Query : IRequest<Result<List<ReviewDto>>>
        {
            public int Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<ReviewDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<List<ReviewDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var reviews = await _context.Reviews
                .Where(r => r.CourtClusterId == request.Id)
                .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

                if (reviews is null)
                    return Result<List<ReviewDto>>.Failure("Review not found");
                return Result<List<ReviewDto>>.Success(reviews);
            }
        }
    }
}
