using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.CourtSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Courts
{
    public class ListByCourtCluster
    {
        public class Query : IRequest<Result<IReadOnlyList<CourtOfClusterDto>>>
        {
            public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IUnitOfWork _unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<IReadOnlyList<CourtOfClusterDto>>>
        {
            public async Task<Result<IReadOnlyList<CourtOfClusterDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecWithFilterParam;
                var spec = new CourtOfCourtClusterSpecification(querySpec);
                var data = await _unitOfWork.Repository<Court>().QueryList(spec)
                .ProjectTo<CourtOfClusterDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
                return Result<IReadOnlyList<CourtOfClusterDto>>.Success(data);
            }
            
        }
    }
}