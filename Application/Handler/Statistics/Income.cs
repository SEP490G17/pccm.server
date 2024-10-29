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
        public class Query : IRequest<Result<IEnumerable<IncomeResult>>>
        {
            public StatisticInputDTO StatisticInput { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<IncomeResult>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<IEnumerable<IncomeResult>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var year = !string.IsNullOrEmpty(request.StatisticInput.Year) && request.StatisticInput.Year != "all"
                    ? int.Parse(request.StatisticInput.Year)
                    : DateTime.Now.Year;

                var month = !string.IsNullOrEmpty(request.StatisticInput.Month) && request.StatisticInput.Month != "all"
                    ? int.Parse(request.StatisticInput.Month)
                    : 0;

                // Kiểm tra CourtClusterId
                int? courtClusterId = request.StatisticInput.CourtClusterId; 

                var query = _context.Bookings
                    .Where(b => b.PaymentStatus == PaymentStatus.Paid && b.Status == BookingStatus.Confirmed)
                    .GroupJoin(
                        _context.Orders.Where(o => o.Status == "Đã hoàn thành"),
                        b => b.Id,
                        o => o.BookingId,
                        (b, orders) => new
                        {
                            Booking = b,
                            TotalOrderAmount = orders.Sum(x => x.TotalAmount),
                            TotalBooking = 1
                        }
                    );

                // Thêm điều kiện theo tháng
                if (month > 0)
                {
                    query = query.Where(x => x.Booking.StartTime.Year == year && x.Booking.StartTime.Month == month);
                }
                else
                {
                    query = query.Where(x => x.Booking.StartTime.Year == year);
                }

                // Thêm điều kiện cho CourtClusterId
                if (courtClusterId.HasValue)
                {
                    query = query.Where(x => x.Booking.Court != null &&
                        _context.Courts.Where(c => c.CourtClusterId == courtClusterId.Value)
                        .Select(c => c.Id).Contains(x.Booking.Court.Id)); // Sử dụng Court.Id
                }

                // Nếu tháng đã được chỉ định
                if (month > 0)
                {
                    var daysInMonth = DateTime.DaysInMonth(year, month);
                    var defaultIncomeByDay = Enumerable.Range(1, daysInMonth)
                        .Select(day => new IncomeResult
                        {
                            Date = $"{day:D2}/{month:D2}",
                            TotalAmount = 0,
                            TotalBooking = 0 // Khởi tạo
                        })
                        .ToList();

                    var totalAmountsByDay = await query
                        .GroupBy(x => x.Booking.StartTime.Day)
                        .Select(g => new IncomeResult
                        {
                            Date = $"{g.Key:D2}/{month:D2}",
                            TotalAmount = (g.Sum(x => x.TotalOrderAmount) + g.Sum(x => x.Booking.TotalPrice)) / 1_000_000m,
                            TotalBooking = g.Count() // Tính tổng số booking
                        })
                        .ToListAsync(cancellationToken);

                    foreach (var dayIncome in totalAmountsByDay)
                    {
                        var match = defaultIncomeByDay.FirstOrDefault(d => d.Date == dayIncome.Date);
                        if (match != null)
                        {
                            match.TotalAmount = dayIncome.TotalAmount;
                            match.TotalBooking = dayIncome.TotalBooking; // Cập nhật TotalBooking
                        }
                    }

                    return Result<IEnumerable<IncomeResult>>.Success(defaultIncomeByDay);
                }
                else // Nếu không có tháng
                {
                    var defaultIncomeByMonth = Enumerable.Range(1, 12)
                        .Select(m => new IncomeResult
                        {
                            Date = $"Tháng {m:D2}",
                            TotalAmount = 0,
                            TotalBooking = 0 // Khởi tạo
                        })
                        .ToList();

                    var totalAmountsByMonth = await query
                        .GroupBy(x => x.Booking.StartTime.Month)
                        .Select(g => new IncomeResult
                        {
                            Date = $"Tháng {g.Key:D2}",
                            TotalAmount = (g.Sum(x => x.TotalOrderAmount) + g.Sum(x => x.Booking.TotalPrice)) / 1_000_000m,
                            TotalBooking = g.Count() // Tính tổng số booking
                        })
                        .ToListAsync(cancellationToken);

                    foreach (var monthIncome in totalAmountsByMonth)
                    {
                        var match = defaultIncomeByMonth.FirstOrDefault(d => d.Date == monthIncome.Date);
                        if (match != null)
                        {
                            match.TotalAmount = monthIncome.TotalAmount;
                            match.TotalBooking = monthIncome.TotalBooking; // Cập nhật TotalBooking
                        }
                    }

                    return Result<IEnumerable<IncomeResult>>.Success(defaultIncomeByMonth);
                }
            }

        }
    }
}
