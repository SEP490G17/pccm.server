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
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class ListAllUserSite
    {
        public class Query : IRequest<Result<List<CourtClusterDto.CourtCLusterListPageUserSite>>> { }

        public class Handler(IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<List<CourtClusterDto.CourtCLusterListPageUserSite>>>
        {
            public async Task<Result<List<CourtClusterDto.CourtCLusterListPageUserSite>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var courtCluster = await unitOfWork.Repository<CourtCluster>().QueryList(null)
                .ProjectTo<CourtClusterDto.CourtCLusterListPageUserSite>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
                return Result<List<CourtClusterDto.CourtCLusterListPageUserSite>>.Success(courtCluster);
            }
        }
    }
}