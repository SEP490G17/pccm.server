using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
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
        public class Query : IRequest<Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>> { 
              public BaseSpecWithFilterParam  BaseSpecWithFilterParam { get; set; }

        }

        public class Handler(IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>>
        {
            public async Task<Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>> Handle(Query request, CancellationToken cancellationToken)
            {
                  var querySpec = request.BaseSpecWithFilterParam;

                  var spec = new CourtClustersSpecification(querySpec);
                  var specCount = new CourtClustersCountSpecification(querySpec);
                  var totalElement = await unitOfWork.Repository<CourtCluster>().CountAsync(specCount, cancellationToken);

                  var data = await unitOfWork.Repository<CourtCluster>().QueryList(spec)
                    .Where(c => c.DeleteAt == null && c.IsVisible)
                    .ProjectTo<CourtClusterDto.CourtClusterListPageUserSite>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>.Success(new Pagination<CourtClusterDto.CourtClusterListPageUserSite>(querySpec.PageSize, totalElement, data));
            }
        }

    }
}