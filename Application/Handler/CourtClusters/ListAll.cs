using Application.Core;
using Application.DTOs;
using Application.Handler.StaffPositions;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.CourtClusters
{
    public class ListAll
    {
        public class Query : IRequest<Result<List<CourtClusterDto.CourtClusterListAll>>> { }

        public class Handler(IMapper mapper, IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<Query, Result<List<CourtClusterDto.CourtClusterListAll>>>
        {
            public async Task<Result<List<CourtClusterDto.CourtClusterListAll>>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<int> courtClusterId = await mediator.Send(new GetCurrentStaffCluster.Query(), cancellationToken);
                var courtCluster = await unitOfWork.Repository<CourtCluster>().QueryList(null)
                .Where(x => x.DeleteAt == null && (courtClusterId == null || courtClusterId.Contains(x.Id)))
                .ProjectTo<CourtClusterDto.CourtClusterListAll>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
                return Result<List<CourtClusterDto.CourtClusterListAll>>.Success(courtCluster);
            }
        }
    }
}