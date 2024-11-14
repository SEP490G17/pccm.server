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
        public class Query : IRequest<Result<IReadOnlyList<CourtDto>>>
        {
            public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IUnitOfWork _unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<IReadOnlyList<CourtDto>>>
        {
            public async Task<Result<IReadOnlyList<CourtDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecWithFilterParam;
                var spec = new CourtOfCourtClusterSpecification(querySpec);
                var data = await _unitOfWork.Repository<Court>().QueryList(spec)
                .ProjectTo<CourtDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
                return Result<IReadOnlyList<CourtDto>>.Success(data);
            }
            
        }
    }
}