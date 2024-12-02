using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;
using Application.Handler.CourtClusters;
using Domain.Enum;

namespace Pccm.UnitTest.CourtClusters
{
    public class CreateCourtClusterHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateCourtClusterHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(
    "Cụm sân AA1", "HCM", "TP Hồ Chí Minh", "Q1", "Quận 1", "Phường 1", "Phường 1",
    "123 Đường ABC, Quận 1", "f4a3747c-afa1-4ae2-831e-c4867dc2d3b0",
    "Sân 11", CourtStatus.Available, 200000, "07:00", "22:00",
    ExpectedResult = true)]
        public async Task<bool> Handle_CreateCourtCluster_WhenValid(
            string title,
            string province,
            string provinceName,
            string district,
            string districtName,
            string ward,
            string wardName,
            string address,
            string ownerId,
            string courtName,
            CourtStatus courtStatus,
            decimal courtPrice,
            string courtOpenTime,
            string courtCloseTime)
        {
            try
            {
                // Tạo chi tiết sân
                var courtDetails = new List<CourtDetailsDto>
        {
            new CourtDetailsDto
            {
                CourtName = courtName,
                Status = courtStatus,
                CourtPrice = new List<CourtPricesDto>
                {
                    new CourtPricesDto
                    {
                        Price = courtPrice,
                        FromTime = TimeOnly.Parse(courtOpenTime),
                        ToTime = TimeOnly.Parse(courtCloseTime)
                    }
                }
            }
        };

                // Chuẩn bị dữ liệu cho CourtCluster
                var courtClusterInputDto = new CourtClustersInputDto
                {
                    Title = title,
                    Province = province,
                    ProvinceName = provinceName,
                    District = district,
                    DistrictName = districtName,
                    Ward = ward,
                    WardName = wardName,
                    Address = address,
                    OwnerId = ownerId,
                    OpenTime = new TimeOnly(6, 0),
                    CloseTime = new TimeOnly(22, 0),
                    CreatedAt = DateTime.Now,
                    Description = "Mô tả chi tiết về cụm sân",
                    Images = new string[] { "image1.jpg", "image2.jpg" },
                    CourtDetails = courtDetails
                };

                // Gửi lệnh thông qua Mediator
                var result = await Mediator.Send(new Create.Command { CourtCluster = courtClusterInputDto }, default);

                // Trả về kết quả kiểm tra
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false; // Trả về false nếu có lỗi
            }
        }


        [TestCase("Cụm sân A", "HCM", "TP Hồ Chí Minh", "Q1", "Quận 1", "Phường 1", "Phuong 1", "Ha Nam", "f4a3747c-afa1-4ae2-831e-c4867dc2d3b01", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateCourtClusterFail_WhenNotExistOwnerId(
            string title,
            string province,
            string provinceName,
            string district,
            string districtName,
            string ward,
            string wardName,
            string address,
            string ownerId)
        {
            try
            {
                var courtClusterInputDto = new CourtClustersInputDto
                {
                    Title = title,
                    Province = province,
                    ProvinceName = provinceName,
                    District = district,
                    DistrictName = districtName,
                    Ward = ward,
                    WardName = wardName,
                    Address = address,
                    OwnerId = ownerId,
                    OpenTime = new TimeOnly(6, 0),
                    CloseTime = new TimeOnly(22, 0),
                    CreatedAt = DateTime.Now,
                    Description = "Mô tả chi tiết về cụm sân",
                    Images = new string[] { "image1.jpg", "image2.jpg" }
                };

                var result = await Mediator.Send(new Create.Command { CourtCluster = courtClusterInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(null, "HCM", "TP Hồ Chí Minh", "Q1", "Quận 1", "Phường 1", "Phuong 1", "Ha Nam", "f4a3747c-afa1-4ae2-831e-c4867dc2d3b0", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateCourtClusterFail_WhenTitleIsNull(
           string? title,
           string province,
           string provinceName,
           string district,
           string districtName,
           string ward,
           string wardName,
           string address,
           string ownerId)
        {
            try
            {
                var courtClusterInputDto = new CourtClustersInputDto
                {
                    Title = title,
                    Province = province,
                    ProvinceName = provinceName,
                    District = district,
                    DistrictName = districtName,
                    Ward = ward,
                    WardName = wardName,
                    Address = address,
                    OwnerId = ownerId,
                    OpenTime = new TimeOnly(6, 0),
                    CloseTime = new TimeOnly(22, 0),
                    CreatedAt = DateTime.Now,
                    Description = "Mô tả chi tiết về cụm sân",
                    Images = new string[] { "image1.jpg", "image2.jpg" }
                };

                var result = await Mediator.Send(new Create.Command { CourtCluster = courtClusterInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
