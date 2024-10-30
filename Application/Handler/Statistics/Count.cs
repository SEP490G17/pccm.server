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
                        OrderCount = g.Count()
                    })
                    .OrderByDescending(g => g.OrderCount)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                var topStaffDetails = topStaffs.Select(s =>
                    _context.StaffDetails
                        .Where(staff => staff.Id == s.StaffId)
                        .Include(staff => staff.User) 
                        .Select(staff => new StaffDetail
                        {
                            Id = staff.Id,
                            User = new AppUser
                            {
                                FirstName = staff.User.FirstName,
                                LastName = staff.User.LastName
                            }
                        })
                        .FirstOrDefault()
                ).ToList();

                var topProducts = await _context.OrderDetails
                    .GroupBy(od => od.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalQuantity = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(g => g.TotalQuantity)
                    .Take(5)
                    .Select(g => _context.Products.FirstOrDefault(p => p.Id == g.ProductId))
                    .ToListAsync(cancellationToken);

                var statisticCount = new StatisticCount
                {
                    TotalCourtClusters = totalCourtClusters,
                    TotalCourts = totalCourts,
                    TotalUsers = totalUsers,
                    TotalStaff = totalStaff,
                    TopStaffs = topStaffDetails, 
                    TopProducts = topProducts
                };

                return Result<StatisticCount>.Success(statisticCount);
            }
        }
    }
}
