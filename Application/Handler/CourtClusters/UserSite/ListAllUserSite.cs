using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.CourtClusters
{
    public class ListAllUserSite
    {
        public class Query : IRequest<Result<IReadOnlyList<CourtClusterDto.CourtClusterListPageUserSite>>> { }

        public class Handler(IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<CourtClusterDto.CourtClusterListPageUserSite>>>
        {
            public async Task<Result<IReadOnlyList<CourtClusterDto.CourtClusterListPageUserSite>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var courtCluster = await unitOfWork.Repository<CourtCluster>()
                .QueryList(null)
                .Where(c => c.DeleteAt == null && c.IsVisible)
                .Include(c => c.Courts)
                .ThenInclude(c => c.CourtPrices)
                .Include(c => c.Courts)
                .ThenInclude(c => c.CourtCombos)
                .ProjectTo<CourtClusterDto.CourtClusterListPageUserSite>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
                return Result<IReadOnlyList<CourtClusterDto.CourtClusterListPageUserSite>>.Success(courtCluster);
            }
        }
    }
}