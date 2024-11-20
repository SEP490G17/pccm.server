using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtPrices
{
    public class UpdatePricesOfCourt
    {
        public class Command : IRequest<Result<List<CourtPriceResponseDto>>>
        {

            public int CourtId { get; set; }
            public List<CourtPriceResponseDto> CourtPriceResponseDtos { get; set; }
        }

        public class Handler(DataContext dataContext, IMapper mapper) : IRequestHandler<Command, Result<List<CourtPriceResponseDto>>>
        {
            public async Task<Result<List<CourtPriceResponseDto>>> Handle(Command request, CancellationToken cancellationToken)
            {
                var check = await dataContext.Courts.Include(c => c.CourtPrices).FirstOrDefaultAsync(
                    c => c.Id == request.CourtId && c.DeleteAt == null, cancellationToken);

                if (check == null)
                {
                    return Result<List<CourtPriceResponseDto>>.Failure("Sân không tồn tại");
                }
                check.CourtPrices.Clear();
                check.CourtPrices = mapper.Map<List<CourtPrice>>(request.CourtPriceResponseDtos);
                var courtPrices = await dataContext.CourtPrices.Where(c=>c.Court.Id == request.CourtId).ToListAsync(cancellationToken);
                dataContext.Update(check);
                dataContext.RemoveRange(courtPrices);
                await dataContext.SaveChangesAsync(cancellationToken);
                var result = dataContext.Entry(check);
                await result.Collection(c => c.CourtPrices).LoadAsync();

                return Result<List<CourtPriceResponseDto>>.Success(mapper.Map<List<CourtPriceResponseDto>>(result.Entity.CourtPrices));
            }
        }
    }
}