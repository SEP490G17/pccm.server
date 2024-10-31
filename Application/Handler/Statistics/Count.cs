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

                var statisticCount = new StatisticCount
                {
                    TotalCourtClusters = totalCourtClusters,
                    TotalCourts = totalCourts,
                    TotalUsers = totalUsers,
                    TotalStaff = totalStaff,
                };

                return Result<StatisticCount>.Success(statisticCount);
            }
        }
    }
}
