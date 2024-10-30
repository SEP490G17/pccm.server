using Application.Core;
using Application.DTOs;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Statistics
{
    public class GetYears
    {
        public class Query : IRequest<Result<List<int>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<int>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<int>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var bookingYears = await _context.Bookings
                    .Select(b => b.StartTime.Year)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var orderYears = await _context.Orders
                    .Select(o => o.CreatedAt.Year)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var uniqueYears = bookingYears.Concat(orderYears)
                    .Distinct()
                    .OrderBy(year => year)
                    .ToList();

                return Result<List<int>>.Success(uniqueYears); ;
            }
        }
    }
}
