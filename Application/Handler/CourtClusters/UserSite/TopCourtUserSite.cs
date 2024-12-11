using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
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
                var querySpec = request.BaseSpecWithFilterParam;

                // Lấy thời gian hiện tại và 30 ngày trước
                var today = DateTime.UtcNow.Date; // Chỉ lấy ngày hiện tại
                var thirtyDaysAgo = today.AddDays(-30); // Lấy ngày cách đây 30 ngày

                // Lọc Booking trong 30 ngày qua và tính số lượng theo CourtClusterId
                var topCourtClusterIds = await unitOfWork.Repository<Booking>()
                    .QueryList(null)
                    .Where(b => b.CreatedAt.Date >= thirtyDaysAgo && b.CreatedAt.Date <= today) // So sánh chỉ phần ngày
                    .Join(
                        unitOfWork.Repository<Court>().QueryList(null), // Join với bảng Court
                        booking => booking.Court.Id, // Dùng Court.Id từ navigation property
                        court => court.Id,           // Khớp với Court.Id
                        (booking, court) => new { court.CourtClusterId } // Lấy CourtClusterId từ Court
                    )
                    .Where(x => x.CourtClusterId != null) // Loại bỏ Court không có CourtClusterId
                    .GroupBy(x => x.CourtClusterId) // Nhóm kết quả theo CourtClusterId
                    .Select(group => new
                    {
                        CourtClusterId = group.Key.Value, // CourtClusterId nullable, nên cần Value
                        BookingCount = group.Count()      // Số lượng Booking trong nhóm
                    })
                    .OrderByDescending(x => x.BookingCount) // Sắp xếp giảm dần theo số lượng Booking
                    .Take(3) // Lấy top 3 CourtCluster
                    .Select(x => x.CourtClusterId) // Lấy danh sách CourtClusterId
                    .ToListAsync(cancellationToken);

                // Lấy danh sách CourtCluster tương ứng với các CourtClusterId đã lọc
                var courtClusters = await unitOfWork.Repository<CourtCluster>()
                    .QueryList(null)
                    .Where(c => topCourtClusterIds.Contains(c.Id) && c.DeleteAt == null && c.IsVisible)
                    .ProjectTo<CourtClusterDto.CourtClusterListPageUserSite>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                // Sắp xếp danh sách CourtCluster theo thứ tự trong topCourtClusterIds
                courtClusters = courtClusters
                    .OrderBy(c => topCourtClusterIds.IndexOf(c.Id))  // Sắp xếp theo thứ tự trong topCourtClusterIds
                    .ToList();

                // Vì `Pagination` yêu cầu `totalElement`, sử dụng tổng số cụm sân trong kết quả
                var totalElement = courtClusters.Count();

                return Result<Pagination<CourtClusterDto.CourtClusterListPageUserSite>>.Success(new Pagination<CourtClusterDto.CourtClusterListPageUserSite>(querySpec.PageSize, totalElement, courtClusters));
            }
        }

    }
}