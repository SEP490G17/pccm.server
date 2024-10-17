using Application.Core;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class Detail
    {
          public class Query : IRequest<Result<CourtCluster>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<CourtCluster>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                this._context = context;
            }
            public async Task<Result<CourtCluster>> Handle(Query request, CancellationToken cancellationToken)
            {
                var court = await _context.CourtClusters.FirstOrDefaultAsync(x => x.Id == request.Id);
                
                if (court is null)
                    return Result<CourtCluster>.Failure("Court cluster not found");
                return Result<CourtCluster>.Success(court);
            }
        }
    }
}