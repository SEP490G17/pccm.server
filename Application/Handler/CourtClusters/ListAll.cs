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
    public class ListAll
    {
        public class Query : IRequest<Result<List<CourtClusterDto.CourtClusterListAll>>> { }

        public class Handler(IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<List<CourtClusterDto.CourtClusterListAll>>>
        {
            public async Task<Result<List<CourtClusterDto.CourtClusterListAll>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var courtCluster = await unitOfWork.Repository<CourtCluster>().QueryList(null)
                .ProjectTo<CourtClusterDto.CourtClusterListAll>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
                return Result<List<CourtClusterDto.CourtClusterListAll>>.Success(courtCluster);
            }
        }
    }
}