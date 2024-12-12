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
                            (b.UntilTime == null && b.StartTime.Date == today) || // Booking theo ngày
                            (b.UntilTime.HasValue && b.StartTime.Date <= today && b.UntilTime.Value.Date >= today) // Booking combo
                        )
                        .ToListAsync(cancellationToken);

                    // Lấy danh sách tất cả sân hiện có
                    var allCourts = await unitOfWork.Repository<Court>()
                        .QueryList(null)
                        .Where(c => c.DeleteAt == null && c.Status == CourtStatus.Available) // Chỉ lấy sân chưa bị xóa
                        .ToListAsync(cancellationToken);

                    // Tính thời gian trống cho từng sân
                    var courtFreeHours = allCourts
                        .GroupJoin(
                            bookings,
                            court => court.Id,
                            booking => booking.Court.Id,
                            (court, courtBookings) => new
                            {
                                Court = court,
                                Bookings = courtBookings.ToList()
                            }
                        )
                        .Select(group =>
                        {
                            var court = group.Court;
                            var bookedSlots = group.Bookings
                                .Select(b => new
                                {
                                    Start = b.StartTime.TimeOfDay,
                                    End = b.UntilTime == null ? b.EndTime.TimeOfDay : TimeSpan.FromHours(24) // UntilTime = 24h cho combo
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

                            // Thêm khoảng trống cuối ngày nếu còn
                            if (lastEnd < TimeSpan.FromHours(24))
                            {
                                freeHours += (TimeSpan.FromHours(24) - lastEnd).TotalHours;
                            }

                            // Nếu sân chưa có booking nào, toàn bộ 24 giờ được xem là thời gian trống
                            if (!bookedSlots.Any())
                            {
                                freeHours = 24.0;
                            }

                            return new { CourtId = court.Id, CourtClusterId = court.CourtClusterId, FreeHours = freeHours };
                        })
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
                catch (Exception ex)
                {
                    return Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>.Failure($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
