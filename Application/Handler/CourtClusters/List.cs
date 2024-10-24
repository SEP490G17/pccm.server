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
        public class Query : IRequest<Result<Pagination<CourtCluster>>>
        {
               public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<CourtCluster>>>
        {
            public async Task<Result<Pagination<CourtCluster>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecWithFilterParam;

                var spec = new CourtClustersSpecification(querySpec);
                var specCount = new CourtClustersCountSpecification(querySpec);

                var totalElement = await _unitOfWork.Repository<CourtCluster>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<CourtCluster>().QueryList(spec).ToListAsync(cancellationToken);

                return Result<Pagination<CourtCluster>>.Success(new Pagination<CourtCluster>(querySpec.PageSize, totalElement, data));
            }
        }

    }
}