using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class Detail
    {
        public class Query : IRequest<Result<CourtClusterDto.CourtClusterDetails>>
        {
            public int Id { get; set; }
        }

        public class Handler(DataContext _context, IMapper _mapper)
            : IRequestHandler<Query, Result<CourtClusterDto.CourtClusterDetails>>
        {
            public async Task<Result<CourtClusterDto.CourtClusterDetails>> Handle(
                Query request, CancellationToken cancellationToken)
            {
                var court = await _context.CourtClusters
                    .ProjectTo<CourtClusterDto.CourtClusterDetails>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (court is null)
                    return Result<CourtClusterDto.CourtClusterDetails>.Failure(
                        "Court cluster not found");

                return Result<CourtClusterDto.CourtClusterDetails>.Success(court);
            }
        }
    }
}