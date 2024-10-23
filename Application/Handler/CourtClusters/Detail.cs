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
          public class Query : IRequest<Result<CourtClustersInputDTO>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<CourtClustersInputDTO>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context,IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }
            public async Task<Result<CourtClustersInputDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var court = await _context.CourtClusters.Include(x => x.Services).Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == request.Id);
                if (court is null)
                    return Result<CourtClustersInputDTO>.Failure("Court cluster not found");
                 var courtClusterMap = _mapper.Map<CourtClustersInputDTO>(court);

                return Result<CourtClustersInputDTO>.Success(courtClusterMap);
            }
        }
    }
}