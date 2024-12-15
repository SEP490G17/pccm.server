using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams.CourtClusterSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.CourtClusters
{
    public class ListAllUserSite
    {
        public class Query : IRequest<Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>>
        {
            public CourtClusterSpecParam CourtClusterSpecParam { get; set; }

        }

        public class Handler(IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>>
        {
            public async Task<Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.CourtClusterSpecParam;

                var spec = new CourtClustersUserSiteSpecification(querySpec);
                var specCount = new CourtClustersUserSiteCountSpecification(querySpec);
                var totalElement = await unitOfWork.Repository<CourtCluster>().CountAsync(specCount, cancellationToken);

                var data = await unitOfWork.Repository<CourtCluster>().QueryList(spec)
                  .Where(c => c.DeleteAt == null
                  && c.IsVisible
                  && c.Courts.Count() > 0
                  && c.Courts.Any(c => c.DeleteAt == null && c.Status == Domain.Enum.CourtStatus.Available))
                  .ProjectTo<CourtClusterDto.CourtClusterListPageUserSite>(mapper.ConfigurationProvider)
                  .ToListAsync(cancellationToken);

                return Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>.Success(new Pagination<CourtClusterDto.CourtClusterListPageUserSite>(querySpec.PageSize, totalElement, data));
            }
        }

    }
}