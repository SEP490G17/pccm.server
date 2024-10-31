using Application.Core;
using Application.DTOs;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Handler.Statistics
{
    public class StatisticCluster
    {
        public class Query : IRequest<Result<ClusterStatisticsDto>>
        {
            public DateTime Date { get; set; }
            public int CourtClusterId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ClusterStatisticsDto>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<ClusterStatisticsDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Thống kê các sân thuộc cụm sân đó
                var bookingDetails = await _context.Bookings
                    .Where(b => b.StartTime.Date == request.Date.Date &&
                                 b.Court.CourtClusterId == request.CourtClusterId &&
                                 b.Status == BookingStatus.Confirmed &&
                                 b.PaymentStatus == PaymentStatus.Paid)
                    .GroupBy(b => b.Court.CourtName)
                    .Select(g => new BookingDetailDto
                    {
                        CourtName = g.Key,
                        HoursBooked = g.Sum(b => EF.Functions.DateDiffHour(b.StartTime, b.EndTime)), // Tính tổng HoursBooked
                        TotalPrice = g.Sum(b => b.TotalPrice) // Tính tổng TotalPrice
                    })
                    .ToListAsync(cancellationToken);

                // Thống kê các dịch vụ
                var orderDetails = await _context.OrderDetails
                    .Include(od => od.Order)
                    .Where(od => od.Order.Status == "Đã hoàn thành" &&
                                 _context.Bookings
                                     .Where(b => b.Id == od.Order.BookingId &&
                                                  b.Court.CourtClusterId == request.CourtClusterId &&
                                                  b.Status == BookingStatus.Confirmed &&
                                                  b.PaymentStatus == PaymentStatus.Paid)
                                     .Any() &&
                                 od.Order.CreatedAt.Date == request.Date.Date)
                    .GroupBy(od => od.Product.ProductName)
                    .Select(g => new OrderDetailDto
                    {
                        ProductName = g.Key,
                        Quantity = g.Sum(od => od.Quantity), // Tính tổng Quantity
                        TotalPrice = g.Sum(od => od.TotalPrice) // Tính tổng TotalPrice
                    })
                    .ToListAsync(cancellationToken);

                var statistics = new ClusterStatisticsDto
                {
                    BookingDetails = bookingDetails,
                    OrderDetails = orderDetails,
                };

                return Result<ClusterStatisticsDto>.Success(statistics);
            }
        }
    }
}
