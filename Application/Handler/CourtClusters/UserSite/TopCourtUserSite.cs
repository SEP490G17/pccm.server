using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.CourtClusterSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.CourtClusters.UserSite
{
    public class TopCourtUserSite
    {
        public class Query : IRequest<Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>>
        {
            public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>>
        {
            public async Task<Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var querySpec = request.BaseSpecWithFilterParam;
                    var now = DateTime.UtcNow;
                    var today = now.Date;
                    var endOfDay = today.AddDays(1).AddSeconds(-1); // Kết thúc ngày hiện tại

                    // Lấy danh sách booking trong ngày hiện tại
                    var bookings = await unitOfWork.Repository<Booking>()
                        .QueryList(null)
                        .Where(b =>
                            (b.UntilTime == null && b.StartTime >= today && b.StartTime < endOfDay) || // Booking theo ngày
                            (b.UntilTime.HasValue && b.StartTime <= endOfDay && b.UntilTime.Value >= today)
                        )
                        .ToListAsync(cancellationToken);

                    // Lấy danh sách tất cả sân hiện có
                    var allCourts = await unitOfWork.Repository<Court>()
                        .QueryList(null)
                        .Where(c => c.DeleteAt == null && (int)c.Status == (int)CourtStatus.Available) // Chỉ lấy sân chưa bị xóa
                        .ToListAsync(cancellationToken);

                    var courtFreeHours = allCourts
                                    .GroupJoin(
                                        bookings,
                                        court => court.Id,
                                        booking => booking.Court?.Id, // Ensure the booking has a valid Court reference
                                        (court, courtBookings) => new
                                        {
                                            Court = court,
                                            Bookings = courtBookings?.ToList() ?? new List<Booking>() // Safeguard against null courtBookings
                                        }
                                    )
                                .Select(group =>
                                {
                                    var court = group.Court;
                                    if (court == null) return null; // Skip if court is null

                                    var bookedSlots = group.Bookings
                                        .Where(b => b.StartTime != null) // Filter out bookings with null StartTime
                                        .Select(b => new
                                        {
                                            Start = b.StartTime.TimeOfDay, // Ensure StartTime is not null
                                            End = b.UntilTime?.TimeOfDay ?? TimeSpan.FromHours(24) // Safely handle UntilTime nulls
                                        })
                                        .OrderBy(slot => slot.Start)
                                        .ToList();

                                    var freeHours = 0.0;
                                    var lastEnd = TimeSpan.Zero;

                                    foreach (var slot in bookedSlots)
                                    {
                                        if (slot.Start > lastEnd)
                                        {
                                            freeHours += (slot.Start - lastEnd).TotalHours;
                                        }
                                        lastEnd = TimeSpan.FromTicks(Math.Max(lastEnd.Ticks, slot.End.Ticks));
                                    }

                                    // Add free time at the end of the day
                                    if (lastEnd < TimeSpan.FromHours(24))
                                    {
                                        freeHours += (TimeSpan.FromHours(24) - lastEnd).TotalHours;
                                    }

                                    // If there are no bookings, the entire day is free
                                    if (!bookedSlots.Any())
                                    {
                                        freeHours = 24.0;
                                    }

                                    return new { CourtId = court.Id, CourtClusterId = court.CourtClusterId, FreeHours = freeHours };
                                })
                                .Where(result => result != null) // Ensure no null results
                                .ToList();

                    // Gom nhóm theo cụm sân (CourtClusterId)
                    var topCourtClusters = courtFreeHours
                        .Where(x => x.CourtClusterId != null) // Chỉ lấy cụm sân hợp lệ
                        .GroupBy(x => x.CourtClusterId.Value)
                        .Select(group => new
                        {
                            CourtClusterId = group.Key,
                            TotalFreeHours = group.Sum(x => x.FreeHours)
                        })
                        .OrderByDescending(x => x.TotalFreeHours) // Sắp xếp giảm dần theo số giờ trống
                        .Select(x => x.CourtClusterId)
                        .ToList();

                    // Lấy thông tin cụm sân
                    var courtClusters = await unitOfWork.Repository<CourtCluster>()
                        .QueryList(null)
                        .Where(c => topCourtClusters.Contains(c.Id) && c.DeleteAt == null && c.IsVisible)
                        .ProjectTo<CourtClusterDto.CourtClusterListPageUserSite>(mapper.ConfigurationProvider)
                        .Take(6)
                        .ToListAsync(cancellationToken);

                    // Sắp xếp theo thứ tự trong topCourtClusters
                    courtClusters = courtClusters
                        .OrderBy(c => topCourtClusters.IndexOf(c.Id))
                        .ToList();

                    // Tính tổng số cụm sân trả về
                    var totalElement = courtClusters.Count();

                    return Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>.Success(
                        new Pagination<CourtClusterDto.CourtClusterListPageUserSite>(querySpec.PageSize, totalElement, courtClusters)
                    );
                }
                catch
                {
                    return Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>.Failure(
                        "Fail"
                    );
                }


            }
        }

    }
}