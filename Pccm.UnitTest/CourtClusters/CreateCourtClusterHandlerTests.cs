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
        "San hn",
        "35",
        "Tỉnh Hà Nam",
        "350",
        "Huyện Kim Bảng",
        "13396",
        "Xã Tượng Lĩnh",
        "Ha Nam",
        "b6341ccf-1a22-426c-83bd-21f3f63cd83f",
        ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCreateCourtCluster_WhenValid(
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
                // Arrange: Tạo đối tượng CourtClustersInputDto
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
                    OpenTime = TimeOnly.Parse("06:00:00"),
                    CloseTime = TimeOnly.Parse("21:00:00"),
                    CreatedAt = DateTime.Now,
                    Description = "<p>Mô tả chi tiết sân 1</p>",
                    Images = new string[]
                    {
                "https://res.cloudinary.com/dasy5hwz4/image/upload/v1733323451/wnz6iwifkkhspuocrbug.png",
                "https://res.cloudinary.com/dasy5hwz4/image/upload/v1733323456/victorzbl7hwnkft2y8a.png"
                    },
                    CourtDetails = new List<CourtDetailsDto>
            {
                new CourtDetailsDto
                {
                    CourtName = "San cua ngang",
                    CourtPrice = new List<CourtPricesDto>
                    {
                        new CourtPricesDto
                        {
                            FromTime = TimeOnly.Parse("06:00:00"),
                            ToTime = TimeOnly.Parse("21:00:00"),
                            Price = 20000
                        }
                    },
                    Status = CourtStatus.Available
                }
            }
                };

                // Act: Gửi command Create
                var result = await Mediator.Send(new Create.Command
                {
                    CourtCluster = courtClusterInputDto,
                }, default);

                // Assert: Kiểm tra kết quả
                return result.IsSuccess;
            }
            catch (Exception)
            {
                // Trả về false nếu xảy ra lỗi
                return false;
            }
        }

         [TestCase(
        null,
        "35",
        "Tỉnh Hà Nam",
        "350",
        "Huyện Kim Bảng",
        "13396",
        "Xã Tượng Lĩnh",
        "Ha Nam",
        "b6341ccf-1a22-426c-83bd-21f3f63cd83f",
        ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateCourtClusterFail_WhenInValidTitle(
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
                // Arrange: Tạo đối tượng CourtClustersInputDto
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
                    OpenTime = TimeOnly.Parse("06:00:00"),
                    CloseTime = TimeOnly.Parse("21:00:00"),
                    CreatedAt = DateTime.Now,
                    Description = "<p>Mô tả chi tiết sân 1</p>",
                    Images = new string[]
                    {
                "https://res.cloudinary.com/dasy5hwz4/image/upload/v1733323451/wnz6iwifkkhspuocrbug.png",
                "https://res.cloudinary.com/dasy5hwz4/image/upload/v1733323456/victorzbl7hwnkft2y8a.png"
                    },
                    CourtDetails = new List<CourtDetailsDto>
            {
                new CourtDetailsDto
                {
                    CourtName = "San cua ngang",
                    CourtPrice = new List<CourtPricesDto>
                    {
                        new CourtPricesDto
                        {
                            FromTime = TimeOnly.Parse("06:00:00"),
                            ToTime = TimeOnly.Parse("21:00:00"),
                            Price = 20000
                        }
                    },
                    Status = CourtStatus.Available
                }
            }
                };

                // Act: Gửi command Create
                var result = await Mediator.Send(new Create.Command
                {
                    CourtCluster = courtClusterInputDto,
                }, default);

                // Assert: Kiểm tra kết quả
                return result.IsSuccess;
            }
            catch (Exception)
            {
                // Trả về false nếu xảy ra lỗi
                return false;
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
