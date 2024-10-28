using Application.Core;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Statistics
{
    public class Income
    {
        public class Query : IRequest<Result<decimal[]>> {
            public string? Year { get; set; }
            public string? Month { get; set; }
            public string? CourtClusterId { get; set; }
        }
        public class Handler(DataContext _context) : IRequestHandler<Query, Result<decimal[]>>
        {
            public async Task<Result<decimal[]>> Handle(Query request, CancellationToken cancellationToken)
            {
                var incomeByMonth = new decimal[12];

                // Nhóm các `orders` theo `BookingId` để tính tổng trước khi nối
                var orderSums = _context.Orders
                    .Where(o => o.Status == "Đã hoàn thành")
                    .GroupBy(o => o.BookingId)
                    .Select(g => new
                    {
                        BookingId = g.Key,
                        TotalOrderAmount = g.Sum(o => o.TotalAmount)
                    });

                // Thực hiện nối với `bookings` sau khi tính tổng các `orders`
                var query = orderSums
                    .Join(
                        _context.Bookings.Where(b => b.PaymentStatus == PaymentStatus.Paid && b.Status == BookingStatus.Confirmed),
                        os => os.BookingId,
                        b => b.Id,
                        (os, b) => new { os.TotalOrderAmount, Booking = b }
                    );

                // Thêm các điều kiện lọc tùy chọn
                if (!string.IsNullOrEmpty(request.Year) && request.Year != "all")
                {
                    int year = int.Parse(request.Year);
                    query = query.Where(x => x.Booking.StartTime.Year == year);
                }
                if (!string.IsNullOrEmpty(request.Month) && request.Month != "all")
                {
                    int month = int.Parse(request.Month);
                    query = query.Where(x => x.Booking.StartTime.Month == month);
                }
                if (!string.IsNullOrEmpty(request.CourtClusterId) && request.CourtClusterId != "all")
                {
                    int courtClusterId = int.Parse(request.CourtClusterId);
                    query = query.Where(x => x.Booking.Court.CourtClusterId == courtClusterId);
                }

                // Tính tổng `TotalAmount` cuối cùng cho từng tháng
                var totalAmounts = await query
                    .GroupBy(x => x.Booking.StartTime.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        TotalAmount = g.Sum(x => x.TotalOrderAmount) + g.Sum(x => x.Booking.TotalPrice)
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
