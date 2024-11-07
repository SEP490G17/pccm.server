using Application.Core;
using Application.DTOs;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

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
                    .Include(b => b.Court) 
                    .ToListAsync(cancellationToken); 

                var bookingDetailsGrouped = bookingDetails
                    .GroupBy(b => b.Court.CourtName)
                    .Select(g => new BookingDetailDto
                    {
                        CourtName = g.Key,
                        HoursBooked = FormatHours(g.Sum(b => (b.EndTime - b.StartTime).TotalHours)), 
                        TotalPrice = g.Sum(b => b.TotalPrice)
                    })
                    .ToList();

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
                        Quantity = g.Sum(od => od.Quantity),
                        TotalPrice = g.Sum(od => od.Price*od.Quantity)
                    })
                    .ToListAsync(cancellationToken);

                var statistics = new ClusterStatisticsDto
                {
                    BookingDetails = bookingDetailsGrouped,
                    OrderDetails = orderDetails,
                };

                return Result<ClusterStatisticsDto>.Success(statistics);
            }

            private static string FormatHours(double totalHours)
            {
                // Tính toán số giờ và phút
                int hours = (int)totalHours;
                int minutes = (int)((totalHours - hours) * 60);

                // Trả về chuỗi theo định dạng "xhyy"
                return $"{hours}h{minutes:D2}p";
            }
        }
    }
}
