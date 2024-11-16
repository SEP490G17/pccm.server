using Application.Core;
using Application.DTOs;
using AutoMapper;
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
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<ClusterStatisticsDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Thống kê các sân thuộc cụm sân đó
                var bookingDetails = await _context.Bookings
                  .Where(b => b.StartTime.Month == request.Date.Month &&
                                b.StartTime.Year == request.Date.Year &&
                                 b.Court.CourtClusterId == request.CourtClusterId &&
                                 b.Status == BookingStatus.Confirmed &&
                                 b.Payment.Status == PaymentStatus.Success)
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
                var orderProductDetails = await _context.OrderDetails
                    .Include(od => od.Order)
                    .Where(od => (int)od.Order.Payment.Status == (int)PaymentStatus.Success &&
                                 _context.Bookings
                                     .Where(b => b.Id == od.Order.BookingId &&
                                                b.Court.CourtClusterId == request.CourtClusterId &&
                                                b.Status == BookingStatus.Confirmed &&
                                                b.Payment.Status == PaymentStatus.Success)
                                     .Any() &&
                                 od.Order.CreatedAt.Date.Month == request.Date.Date.Month &&
                                 od.Order.CreatedAt.Date.Year == request.Date.Date.Year
                                 && od.ProductId != null && od.ServiceId == null)
                    .GroupBy(od => od.Product.ProductName)
                    .Select(g => new OrderProductDetailDto
                    {
                        ProductName = g.Key,
                        Quantity = g.Sum(od => od.Quantity),
                        TotalPrice = g.Sum(od => od.Price * od.Quantity)
                    })
                    .ToListAsync(cancellationToken);

                var orderServiceDetails = await _context.OrderDetails
                    .Include(od => od.Order)
                    .Where(od => (int)od.Order.Payment.Status == (int)PaymentStatus.Success &&
                                 _context.Bookings
                                     .Where(b => b.Id == od.Order.BookingId &&
                                                  b.Court.CourtClusterId == request.CourtClusterId &&
                                                  b.Status == BookingStatus.Confirmed &&
                                                  b.Payment.Status == PaymentStatus.Success)
                                     .Any() &&
                                 od.Order.CreatedAt.Date == request.Date.Date
                                 && od.ServiceId != null && od.ProductId == null)
                    .GroupBy(od => od.Service.ServiceName)
                    .Select(g => new OrderServiceDetailDto
                    {
                        ServiceName = g.Key,
                        Quantity = g.Sum(od => od.Quantity),
                        TotalPrice = g.Sum(od => od.Price * od.Quantity)
                    })
                    .ToListAsync(cancellationToken);

                var expenses = await _context.Expenses.Where(e => e.ExpenseAt.Date.Month == request.Date.Date.Month &&
                                 e.ExpenseAt.Date.Year == request.Date.Date.Year && e.CourtClusterId == request.CourtClusterId).ToListAsync();
                List<ExpenseDetailDto> expenseDetails = _mapper.Map<List<ExpenseDetailDto>>(expenses);
                var statistics = new ClusterStatisticsDto
                {
                    BookingDetails = bookingDetailsGrouped,
                    OrderProductDetails = orderProductDetails,
                    OrderServiceDetails = orderServiceDetails,
                    ExpenseDetails = expenseDetails
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
