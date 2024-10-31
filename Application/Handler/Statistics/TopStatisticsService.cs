using Application.Core;
using Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Handler.Statistics
{
    public class TopStatisticsService
    {
        public class Query : IRequest<Result<TopStatisticDto>>
        {
            public string Month { get; set; }
            public string Year { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<TopStatisticDto>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<TopStatisticDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                int.TryParse(request.Month, out var month);
                int.TryParse(request.Year, out var year);

                // Lấy top 5 nhân viên có số lượng đơn hàng nhiều nhất theo tháng và năm
                var topStaffs = await _context.Orders
                    .Where(o => o.CreatedAt.Month == month && o.CreatedAt.Year == year)
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
                        .FirstOrDefault()
                ).ToList();

                // Lấy top 5 sản phẩm bán chạy nhất theo tháng và năm
                var topProducts = await _context.OrderDetails
                    .Where(od => od.Order.CreatedAt.Month == month && od.Order.CreatedAt.Year == year)
                    .GroupBy(od => od.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalQuantity = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(g => g.TotalQuantity)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                var topProductDetails = topProducts.Select(p =>
                    _context.Products
                        .Where(product => product.Id == p.ProductId)
                        .FirstOrDefault()
                ).ToList();

                // Đưa dữ liệu vào DTO
                var topStatisticDto = new TopStatisticDto
                {
                    TopStaffs = topStaffDetails,
                    TopProducts = topProductDetails
                };

                return Result<TopStatisticDto>.Success(topStatisticDto);
            }
        }
    }
}
