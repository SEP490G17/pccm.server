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
    public class List
    {
        public class Query : IRequest<Result<Pagination<CourtClusterDto.CourtCLusterListPage>>>
        {
               public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<CourtClusterDto.CourtCLusterListPage>>>
        {
            public async Task<Result<Pagination<CourtClusterDto.CourtCLusterListPage>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecWithFilterParam;

                var spec = new CourtClustersSpecification(querySpec);
                var specCount = new CourtClustersCountSpecification(querySpec);

                var totalElement = await _unitOfWork.Repository<CourtCluster>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<CourtCluster>().QueryList(spec)
                   .ProjectTo<CourtClusterDto.CourtCLusterListPage>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                return Result<Pagination<CourtClusterDto.CourtCLusterListPage>>.Success(new Pagination<CourtClusterDto.CourtCLusterListPage>(querySpec.PageSize, totalElement, data));
            }
        }

    }
}
