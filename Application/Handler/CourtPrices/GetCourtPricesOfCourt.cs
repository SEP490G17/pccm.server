using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtPrices
{
    public class GetCourtPricesOfCourt
    {
        public class Query : IRequest<Result<List<CourtPriceResponseDto>>>
        {
            public int CourtId { get; set; }
        }

        public class Handler(DataContext dataContext, IMapper mapper) : IRequestHandler<Query, Result<List<CourtPriceResponseDto>>>
        {
            public async Task<Result<List<CourtPriceResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var check = await dataContext.Courts.FirstOrDefaultAsync(c => c.Id == request.CourtId && c.DeleteAt != null, cancellationToken);
                
                if (check == null)
                {
                    return Result<List<CourtPriceResponseDto>>.Failure("Sân không tồn tại");
                }

                var courtPrices = await dataContext.CourtPrices
                        .Where(c => c.Court.Id == request.CourtId)
                        .ProjectTo<CourtPriceResponseDto>(mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);

                return Result<List<CourtPriceResponseDto>>.Success(courtPrices);
            }
        }
    }
}