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
        public class Query : IRequest<Result<CourtClusterDto.CourtClusterListPageUserSite>>
        {
            public int Id { get; set; }
        }

        public class Handler(DataContext _context, IMapper _mapper)
            : IRequestHandler<Query, Result<CourtClusterDto.CourtClusterListPageUserSite>>
        {
            public async Task<Result<CourtClusterDto.CourtClusterListPageUserSite>> Handle(
                Query request, CancellationToken cancellationToken)
            {
                var courtcluster = await _context.CourtClusters
                    .ProjectTo<CourtClusterDto.CourtClusterListPageUserSite>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var courts = await _context.Courts
                    .Where(x => x.CourtCluster.Id == request.Id)
                    .ProjectTo<CourtOfClusterDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                    courtcluster.Courts = courts;
                    courtcluster.NumbOfCourts = courts.Count();
                return Result<CourtClusterDto.CourtClusterListPageUserSite>.Success(courtcluster);
            }
        }
    }
}