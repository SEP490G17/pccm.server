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
        public class Query : IRequest<Result<CourtClusterDto.CourtCLusterDetails>>
        {
            public int Id { get; set; }
        }

        public class Handler(DataContext _context, IMapper _mapper)
            : IRequestHandler<Query, Result<CourtClusterDto.CourtCLusterDetails>>
        {
            public async Task<Result<CourtClusterDto.CourtCLusterDetails>> Handle(
                Query request, CancellationToken cancellationToken)
            {
                var court = await _context.CourtClusters
                    .ProjectTo<CourtClusterDto.CourtCLusterDetails>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (court is null)
                    return Result<CourtClusterDto.CourtCLusterDetails>.Failure(
                        "Court cluster not found");
                var courtClusterMap = _mapper.Map<CourtClusterDto.CourtCLusterDetails>(court);

                return Result<CourtClusterDto.CourtCLusterDetails>.Success(courtClusterMap);
            }
        }
    }
}