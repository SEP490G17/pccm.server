using Application.Core;
using Application.DTOs;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Statistics
{
    public class Income
    {
        public class Query : IRequest<Result<decimal[]>>
        {
            public FilterStatisticDTO datafilter { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<decimal[]>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<decimal[]>> Handle(Query request, CancellationToken cancellationToken)
            {
                var incomeByMonth = new decimal[12];

                var query = _context.Bookings
                    .Where(b => b.PaymentStatus == PaymentStatus.Paid && b.Status == BookingStatus.Confirmed)
                    .GroupJoin(
                        _context.Orders.Where(o => o.Status == "Đã hoàn thành"),
                        b => b.Id,
                        o => o.BookingId,
                        (b, orders) => new
                        {
                            Booking = b,
                            TotalOrderAmount = orders.Sum(x => x.TotalAmount)
                        }
                    );

                // Thêm các điều kiện lọc tùy chọn
                if (!string.IsNullOrEmpty(request.datafilter.Year) && request.datafilter.Year != "all")
                {
                    int year = int.Parse(request.datafilter.Year);
                    query = query.Where(x => x.Booking.StartTime.Year == year);
                }
                if (!string.IsNullOrEmpty(request.datafilter.Month) && request.datafilter.Month != "all")
                {
                    int month = int.Parse(request.datafilter.Month);
                    query = query.Where(x => x.Booking.StartTime.Month == month);
                }
                if (!string.IsNullOrEmpty(request.datafilter.CourtClusterId) && request.datafilter.CourtClusterId != "all")
                {
                    int courtClusterId = int.Parse(request.datafilter.CourtClusterId);
                    query = query.Where(x => x.Booking.Court.CourtClusterId == courtClusterId);
                }

                // Tính tổng `TotalAmount` cuối cùng cho từng tháng, bao gồm `Booking.TotalPrice`
                var totalAmounts = await query
                    .GroupBy(x => x.Booking.StartTime.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        TotalAmount = (g.Sum(x => x.TotalOrderAmount) + g.Sum(x => x.Booking.TotalPrice)) / 1_000_000m
                    })
                    .ToListAsync(cancellationToken);

                // Đưa kết quả vào mảng `incomeByMonth`
                foreach (var item in totalAmounts)
                {
                    if (item.Month >= 1 && item.Month <= 12)
                        incomeByMonth[item.Month - 1] = item.TotalAmount;
                }

                return Result<decimal[]>.Success(incomeByMonth);
            }
        }
    }
}
