using Application.Core;
using Application.DTOs;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Statistics
{
    public class Count
    {
        public class Query : IRequest<Result<StatisticCount>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<StatisticCount>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<StatisticCount>> Handle(Query request, CancellationToken cancellationToken)
            {
                var totalCourtClusters = await _context.CourtClusters.CountAsync(cancellationToken);
                var totalCourts = await _context.Courts.CountAsync(cancellationToken);
                var totalUsers = await _context.Users.CountAsync(u => !u.IsDisabled, cancellationToken);
                var totalStaff = await _context.StaffDetails.CountAsync(cancellationToken);

                var topStaffs = await _context.Orders
                    .GroupBy(o => o.CreatedBy)
                    .Select(g => new
                    {
                        StaffId = g.Key,
                        FullName = _context.StaffDetails
                            .Where(s => s.Id == g.Key)
                            .Join(_context.Users,
                                s => s.UserId,
                                u => u.Id,
                                (s, u) => u.FirstName + " " + u.LastName)
                            .FirstOrDefault(),
                        OrderCount = g.Count() 
                    })
                    .OrderByDescending(g => g.OrderCount) 
                    .Take(5)
                    .ToListAsync(cancellationToken);

                var topProducts = await _context.OrderDetails
                    .GroupBy(od => od.ProductId)
                    .OrderByDescending(g => g.Sum(od => od.Quantity))
                    .Take(5)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        ProductName = _context.Products
                            .Where(p => p.Id == g.Key)
                            .Select(p => p.ProductName)
                            .FirstOrDefault()
                    })
                    .ToListAsync(cancellationToken);

                var statisticCount = new StatisticCount
                {
                    TotalCourtClusters = totalCourtClusters,
                    TotalCourts = totalCourts,
                    TotalUsers = totalUsers,
                    TotalStaff = totalStaff,
                    TopStaffs = topStaffs.Select(s => s.FullName).ToArray(),
                    TopProducts = topProducts.Select(p => p.ProductName).ToArray() 
                };

                return Result<StatisticCount>.Success(statisticCount);
            }
        }
    }
}
