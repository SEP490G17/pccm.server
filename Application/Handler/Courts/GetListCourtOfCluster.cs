

using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Courts
{
    public class GetListCourtOfCluster
    {
        public class Query : IRequest<Result<CourtManagerResponse>>
        {
            public int CourtClusterId { get; set; }
        }

        public class Handler(DataContext dataContext, IMapper mapper) : IRequestHandler<Query, Result<CourtManagerResponse>>
        {
            public async Task<Result<CourtManagerResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var check = await dataContext.CourtClusters.FirstOrDefaultAsync(c => c.Id == request.CourtClusterId);
                if (check == null)
                {
                    return Result<CourtManagerResponse>.Failure("Cụm sân không tồn tại");
                }
                var courts = await dataContext.Courts
                    .Where(c => c.CourtClusterId.Equals(request.CourtClusterId) && c.DeleteAt == null && c.Status == CourtStatus.Available)
                    .ProjectTo<CourtOfClusterDto>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<CourtManagerResponse>.Success(new CourtManagerResponse
                {
                    CourtClusterName = check.CourtClusterName,
                    OpenTime = check.OpenTime,
                    CloseTime = check.CloseTime,
                    CourtForTable = courts
                });
            }
        }
    }
}