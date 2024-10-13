using Application.Core;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class List
    {
        public class Query : IRequest<Result<List<CourtCluster>>> { }

        public class Handler : IRequestHandler<Query, Result<List<CourtCluster>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<CourtCluster>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var courtCluster = await _context.CourtClusters.ToListAsync(cancellationToken);
                return Result<List<CourtCluster>>.Success(courtCluster);
            }
        }

    }
}