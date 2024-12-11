using Application.Core;
using Application.DTOs;
using Application.Handler.StaffPositions;
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
    public class List
    {
        public class Query : IRequest<Result<Pagination<CourtClusterDto.CourtCLusterListPage>>>
        {
               public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork, IMediator mediator) : IRequestHandler<Query, Result<Pagination<CourtClusterDto.CourtCLusterListPage>>>
        {
            public async Task<Result<Pagination<CourtClusterDto.CourtCLusterListPage>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecWithFilterParam;
                List<int> courtClusterId = await mediator.Send(new GetCurrentStaffCluster.Query(),cancellationToken);
                var spec = new CourtClustersSpecification(querySpec, courtClusterId);
                var specCount = new CourtClustersCountSpecification(querySpec, courtClusterId);

                var totalElement = await _unitOfWork.Repository<CourtCluster>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<CourtCluster>().QueryList(spec)
                   .ProjectTo<CourtClusterDto.CourtCLusterListPage>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                return Result<Pagination<CourtClusterDto.CourtCLusterListPage>>.Success(new Pagination<CourtClusterDto.CourtCLusterListPage>(querySpec.PageSize, totalElement, data));
            }
        }

    }
}
