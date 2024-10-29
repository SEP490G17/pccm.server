using Application.Core;
using Application.DTOs;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Statistics
{
    public class StatisticService
    {
        public class Query : IRequest<Result<IEnumerable<StatisticResult>>>
        {
            public StatisticInputDTO StatisticInput { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<IEnumerable<StatisticResult>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<IEnumerable<StatisticResult>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var (year, month, courtClusterId) = ParseInputs(request.StatisticInput);

                var bookings = await GetBookings(year, month, courtClusterId, cancellationToken);

                var totalAmounts = await GetTotalAmounts(bookings);

                var totalImportFees = await GetTotalImportFees(year, month, courtClusterId, cancellationToken);

                return month > 0
                    ? await BuildDailyStatistics(totalAmounts, totalImportFees, year, month)
                    : await BuildMonthlyStatistics(totalAmounts, totalImportFees);
            }

            private (int year, int month, int? courtClusterId) ParseInputs(StatisticInputDTO input)
            {
                var year = !string.IsNullOrEmpty(input.Year) && input.Year != "all"
                    ? int.Parse(input.Year)
                    : DateTime.Now.Year;

                var month = !string.IsNullOrEmpty(input.Month) && input.Month != "all"
                    ? int.Parse(input.Month)
                    : 0;

                int? courtClusterId = input.CourtClusterId;

                return (year, month, courtClusterId);
            }

            private async Task<List<Booking>> GetBookings(int year, int month, int? courtClusterId, CancellationToken cancellationToken)
            {
                var bookingsQuery = _context.Bookings
                    .Where(b => b.PaymentStatus == PaymentStatus.Paid && b.Status == BookingStatus.Confirmed)
                    .AsQueryable();

                if (month > 0)
                {
                    bookingsQuery = bookingsQuery.Where(b => b.StartTime.Year == year && b.StartTime.Month == month);
                }
                else
                {
                    bookingsQuery = bookingsQuery.Where(b => b.StartTime.Year == year);
                }

                if (courtClusterId.HasValue)
                {
                    bookingsQuery = bookingsQuery.Where(b => b.Court != null && b.Court.CourtClusterId == courtClusterId.Value);
                }

                return await bookingsQuery.ToListAsync(cancellationToken);
            }

            private async Task<IEnumerable<dynamic>> GetTotalAmounts(List<Booking> bookings)
            {
                return bookings
                    .GroupJoin(
                        _context.Orders.Where(o => o.Status == "Đã hoàn thành").ToList(),
                        b => b.Id,
                        o => o.BookingId,
                        (b, orders) => new
                        {
                            Booking = b,
                            TotalOrderAmount = orders.Sum(x => (decimal)x.TotalAmount), // Ensure TotalAmount is treated as decimal
                            TotalBooking = 1
                        }
                    );
            }
            private async Task<List<dynamic>> GetTotalImportFees(int year, int month, int? courtClusterId, CancellationToken cancellationToken)
            {
                var totalImportFeeQuery = _context.Products.AsQueryable();

                if (courtClusterId.HasValue)
                {
                    totalImportFeeQuery = totalImportFeeQuery.Where(p => p.CourtClusterId == courtClusterId.Value);
                }

                if (month > 0)
                {
                    totalImportFeeQuery = totalImportFeeQuery.Where(p => p.CreatedAt.Month == month && p.CreatedAt.Year == year);
                }
                else
                {
                    totalImportFeeQuery = totalImportFeeQuery.Where(p => p.CreatedAt.Year == year);
                }

                return await totalImportFeeQuery
                    .GroupBy(p => p.CreatedAt.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        TotalImportFee = g.Sum(p => p.ImportFee * p.Quantity) / 1_000_000m
                    })
                    .Cast<dynamic>() // Chuyển đổi sang dynamic
                    .ToListAsync(cancellationToken);
            }

            private async Task<Result<IEnumerable<StatisticResult>>> BuildDailyStatistics(
                IEnumerable<dynamic> totalAmounts,
                List<dynamic> totalImportFees,
                int year,
                int month)
            {
                var daysInMonth = DateTime.DaysInMonth(year, month);
                var defaultIncomeByDay = Enumerable.Range(1, daysInMonth)
                    .Select(day => new StatisticResult
                    {
                        Date = $"{day:D2}/{month:D2}",
                        TotalAmount = 0m,  // Đảm bảo kiểu là decimal
                        TotalBooking = 0,
                        TotalImportFee = 0m  // Đảm bảo kiểu là decimal
                    })
                    .ToList();

                var totalAmountsByDay = totalAmounts
                    .GroupBy(x => x.Booking.StartTime.Day)
                    .Select(g => new StatisticResult
                    {
                        Date = $"{g.Key:D2}/{month:D2}",
                        TotalAmount = (g.Sum(x => (decimal)x.TotalOrderAmount) + g.Sum(x => (decimal)x.Booking.TotalPrice)) / 1_000_000m,  // Đảm bảo kiểu là decimal
                        TotalBooking = g.Count(),
                        TotalImportFee = totalImportFees.FirstOrDefault(x => x.Month == month)?.TotalImportFee ?? 0m  // Đảm bảo kiểu là decimal
                    })
                    .ToList();

                foreach (var dayIncome in totalAmountsByDay)
                {
                    var match = defaultIncomeByDay.FirstOrDefault(d => d.Date == dayIncome.Date);
                    if (match != null)
                    {
                        match.TotalAmount = dayIncome.TotalAmount;
                        match.TotalBooking = dayIncome.TotalBooking;
                        match.TotalImportFee = dayIncome.TotalImportFee;
                    }
                }

                return Result<IEnumerable<StatisticResult>>.Success(defaultIncomeByDay);
            }


            private async Task<Result<IEnumerable<StatisticResult>>> BuildMonthlyStatistics(
                IEnumerable<dynamic> totalAmounts,
                List<dynamic> totalImportFees)
            {
                var defaultIncomeByMonth = Enumerable.Range(1, 12)
                    .Select(m => new StatisticResult
                    {
                        Date = $"Tháng {m:D2}",
                        TotalAmount = 0m,  // Ensure this is decimal
                        TotalBooking = 0,
                        TotalImportFee = totalImportFees.FirstOrDefault(x => x.Month == m)?.TotalImportFee ?? 0m  // Ensure this is decimal
                    })
                    .ToList();

                var totalAmountsByMonth = totalAmounts
                    .GroupBy(x => x.Booking.StartTime.Month)
                    .Select(g => new StatisticResult
                    {
                        Date = $"Tháng {g.Key:D2}",
                        TotalAmount = (g.Sum(x => (decimal)x.TotalOrderAmount) + g.Sum(x => (decimal)x.Booking.TotalPrice)) / 1_000_000m,
                        TotalBooking = g.Count(),
                        TotalImportFee = totalImportFees.FirstOrDefault(x => x.Month == g.Key)?.TotalImportFee ?? 0m  // Ensure this is decimal
                    })
                    .ToList();

                foreach (var monthIncome in totalAmountsByMonth)
                {
                    var match = defaultIncomeByMonth.FirstOrDefault(d => d.Date == monthIncome.Date);
                    if (match != null)
                    {
                        match.TotalAmount = monthIncome.TotalAmount;
                        match.TotalBooking = monthIncome.TotalBooking;
                        match.TotalImportFee = monthIncome.TotalImportFee;
                    }
                }

                return Result<IEnumerable<StatisticResult>>.Success(defaultIncomeByMonth);
            }

        }
    }
}
