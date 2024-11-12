using Application.Core;
using Application.DTOs;
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
                var currentDate = DateTime.Now;

                var newUser = await _context.Users
                .Where(u => u.JoiningDate.HasValue &&
                u.JoiningDate.Value.Year == currentDate.Year &&
                u.JoiningDate.Value.Month == currentDate.Month)
                .CountAsync(cancellationToken);

                var totalBookingToday = await _context.Bookings
                .Where(u =>
                u.StartTime.Day == currentDate.Day &&
                u.StartTime.Year == currentDate.Year &&
                u.StartTime.Month == currentDate.Month)
                .CountAsync(cancellationToken);

                var totalBookingMonth = await _context.Bookings
                .Where(u =>
                u.StartTime.Year == currentDate.Year &&
                u.StartTime.Month == currentDate.Month)
                .CountAsync(cancellationToken);

                var productInMonth = await _context.Orders
                .Where(u => u.CreatedAt.Month == currentDate.Month
                && u.CreatedAt.Year == currentDate.Year)
                .SelectMany(u => u.OrderDetails)
                .CountAsync(od => od.ProductId != null, cancellationToken);

                var serviceInMonth = await _context.Orders
                .Where(u => u.CreatedAt.Month == currentDate.Month
                && u.CreatedAt.Year == currentDate.Year)
                .SelectMany(u => u.OrderDetails)
                .CountAsync(od => od.ServiceId != null, cancellationToken);

                var statisticCount = new StatisticCount
                {
                    newUser = newUser,
                    totalBookingMonth = totalBookingMonth,
                    totalBookingToday = totalBookingToday,
                    productInMonth = productInMonth,
                    serviceInMonth = serviceInMonth,
                };

                return Result<StatisticCount>.Success(statisticCount);
            }
        }
    }
}
