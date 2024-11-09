using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class DetailUserSite
    {
        public class Query : IRequest<Result<CourtClusterDto.CourtCLusterListPageUserSite>>
        {
            public int Id { get; set; }
        }

        public class Handler(DataContext _context, IMapper _mapper)
            : IRequestHandler<Query, Result<CourtClusterDto.CourtCLusterListPageUserSite>>
        {
            public async Task<Result<CourtClusterDto.CourtCLusterListPageUserSite>> Handle(
                Query request, CancellationToken cancellationToken)
            {
                var court = await _context.CourtClusters
                    .Include(x => x.Services)
                    .Include(x => x.Products)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var numberOfCourts = await _context.Courts.Where(c => c.CourtCluster.Id == request.Id).ToListAsync();
                var reviews = await _context.Reviews
                .Where(r => r.CourtClusterId == request.Id)
                .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
                if (court is null)
                    return Result<CourtClusterDto.CourtCLusterListPageUserSite>.Failure(
                        "Court cluster not found");
                var courtClusterMap = _mapper.Map<CourtClusterDto.CourtCLusterListPageUserSite>(court);
                courtClusterMap.NumbOfCourts = numberOfCourts.Count();
                courtClusterMap.Reviews = reviews;
                return Result<CourtClusterDto.CourtCLusterListPageUserSite>.Success(courtClusterMap);
            }
        }
    }
}