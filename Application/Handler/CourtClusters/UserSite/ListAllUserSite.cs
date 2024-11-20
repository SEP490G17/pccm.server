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
        public class Query : IRequest<Result<List<CourtClusterDto.CourtClusterListPageUserSite>>> { }

        public class Handler(IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<List<CourtClusterDto.CourtClusterListPageUserSite>>>
        {
            public async Task<Result<List<CourtClusterDto.CourtClusterListPageUserSite>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var courtCluster = await unitOfWork.Repository<CourtCluster>()
                .QueryList(null)
                .Include(c => c.Courts)
                .ProjectTo<CourtClusterDto.CourtClusterListPageUserSite>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
                return Result<List<CourtClusterDto.CourtClusterListPageUserSite>>.Success(courtCluster);
            }
        }
    }
}