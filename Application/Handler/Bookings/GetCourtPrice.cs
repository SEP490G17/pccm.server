using Application.Core;
using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class GetCourtPrice
    {
        public class Query : IRequest<Result<List<PriceCourtDto>>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<PriceCourtDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }
            public async Task<Result<List<PriceCourtDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var prices = await _context.CourtPrices
                .Include(c => c.Court)
                .Where(p => p.Court.CourtClusterId == request.Id && p.Court.DeleteAt == null && p.Court.Status == 0)
                .OrderBy(p => p.Court.CourtName)
                .ThenBy(p => p.FromTime)
                .ToListAsync();

                if (prices.Count == 0) return Result<List<PriceCourtDto>>.Failure("Booking not found");
                var data = _mapper.Map<List<PriceCourtDto>>(prices);
                return Result<List<PriceCourtDto>>.Success(data);
            }
        }
    }
}