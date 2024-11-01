using Application.Core;
using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class Detail
    {
        public class Query : IRequest<Result<CourtClustersInputDto>>
        {
            public int Id { get; set; }
        }

        public class Handler(DataContext _context, IMapper _mapper)
            : IRequestHandler<Query, Result<CourtClustersInputDto>>
        {
            public async Task<Result<CourtClustersInputDto>> Handle(
                Query request, CancellationToken cancellationToken)
            {
                var court = await _context.CourtClusters
                    .Include(x => x.Services)
                    .Include(x => x.Products)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (court is null)
                    return Result<CourtClustersInputDto>.Failure(
                        "Court cluster not found");
                var courtClusterMap = _mapper.Map<CourtClustersInputDto>(court);

                return Result<CourtClustersInputDto>.Success(courtClusterMap);
            }
        }
    }
}