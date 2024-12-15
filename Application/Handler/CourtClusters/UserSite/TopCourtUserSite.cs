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
                    var now = DateTime.UtcNow; // Thời gian hiện tại
                    var today = now.Date; // Bắt đầu ngày hiện tại
                    var endOfDay = today.AddDays(1); // Kết thúc ngày hiện tại
                    var courts = await unitOfWork.Repository<Court>()
                        .QueryList(null)
                        .Where(c => c.DeleteAt == null && (int)c.Status == (int)CourtStatus.Available)
                        .Include(c => c.CourtCluster) // Load thông tin CourtCluster để lấy OpenTime và CloseTime
                        .ToListAsync();

                    var bookings = await unitOfWork.Repository<Booking>()
                        .QueryList(null)
                        .Include(b => b.Court)
                        .Where(b => b.StartTime.Date == today) // Chỉ lấy booking trong ngày hôm nay
                        .ToListAsync();

                    var courtFreeHoursQuery = courts
                        .Where(court => court.CourtCluster != null) // Loại bỏ sân không có CourtCluster
                        .Select(court =>
                        {
                            // Lấy giờ mở cửa và đóng cửa từ cụm sân
                            var openTime = court.CourtCluster.OpenTime; // Mặc định giờ mở cửa là 00:00 nếu null
                            var closeTime = court.CourtCluster.CloseTime ; // Mặc định giờ đóng cửa là 24:00 nếu null

                            // Chuyển giờ mở cửa và đóng cửa thành thời gian thực trong ngày
                            var openingDateTime = today.Add(openTime.ToTimeSpan());
                            var closingDateTime = today.Add(closeTime.ToTimeSpan());

                            // Tổng thời gian hoạt động trong ngày (trong khoảng mở cửa -> đóng cửa)
                            var totalOperatingHours = (closingDateTime - openingDateTime).TotalHours;

                            // Tổng thời gian đã được đặt sân, giới hạn trong khoảng giờ mở/đóng cửa
                            var totalBookedHours = bookings
                                .Where(b => b.Court.Id == court.Id)
                                .Sum(b =>
                                {
                                    // Giới hạn thời gian đặt sân trong khoảng mở cửa - đóng cửa
                                    var bookingStart = b.StartTime < openingDateTime ? openingDateTime : b.StartTime;
                                    var bookingEnd = b.UntilTime > closingDateTime ? closingDateTime : b.UntilTime ?? closingDateTime;

                                    // Nếu thời gian đặt nằm ngoài khoảng mở/đóng cửa, không tính
                                    if (bookingEnd <= bookingStart) return 0;

                                    return (bookingEnd - bookingStart).TotalHours;
                                });

                            // Giờ trống = tổng giờ hoạt động - tổng giờ đã đặt
                            var freeHours = Math.Max(0, totalOperatingHours - totalBookedHours);

                            return new
                            {
                                Court = court,
                                FreeHours = freeHours
                            };
                        })
                        .Where(result => result.FreeHours > 0) // Lọc các sân có giờ trống > 0
                        .ToList();

                    // Gom nhóm theo cụm sân (CourtClusterId) và tính tổng giờ trống
                    var courtFreeHours = courtFreeHoursQuery
                        .Where(x => x.Court.CourtClusterId != null) // Chỉ lấy cụm sân hợp lệ
                        .GroupBy(x => x.Court.CourtClusterId.Value)
                        .Select(group => new
                        {
                            CourtClusterId = group.Key,
                            TotalFreeHours = group.Sum(x => x.FreeHours)
                        })
                        .OrderByDescending(x => x.TotalFreeHours) // Sắp xếp giảm dần theo giờ trống
                        .Take(6) // Lấy tối đa 6 cụm sân
                        .ToList();

                    // Lấy thông tin cụm sân tương ứng với topCourtClusters
                    var topCourtClusters = courtFreeHours.Select(x => x.CourtClusterId).ToList();
                    var courtClusters = await unitOfWork.Repository<CourtCluster>()
                        .QueryList(null)
                        .Where(c => topCourtClusters.Contains(c.Id) && c.DeleteAt == null && c.IsVisible)
                        .ProjectTo<CourtClusterDto.CourtClusterListPageUserSite>(mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);

                    // Sắp xếp lại danh sách theo thứ tự trong topCourtClusters
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