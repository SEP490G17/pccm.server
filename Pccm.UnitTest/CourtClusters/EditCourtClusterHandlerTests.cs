using API.Extensions;
using Application.DTOs;
using Application.Handler.CourtClusters;
using Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.CourtClusters
{
    public class EditCourtClusterHandlerTests
    {
        private readonly IMediator Mediator;

        public EditCourtClusterHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }

        [TestCase(
                 4,
                 "Sân pickleball",
                 "83",
                 "Tỉnh Bến Tre",
                 "834",
                 "Huyện Giồng Trôm",
                 "29047",
                 "Xã Hưng Lễ",
                 "Ngõ 182",
                 "b6341ccf-1a22-426c-83bd-21f3f63cd83f",
                 ExpectedResult = true)]
        public async Task<bool> Handle_ShouldEditCourtCluster_WhenValid(
                 int id,
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
                    OpenTime = TimeOnly.Parse("05:00:00"),
                    CloseTime = TimeOnly.Parse("22:00:00"),
                    CreatedAt = DateTime.Now,
                    Description = "<p>sân</p>",
                    Images = new string[]
                    {
                "https://res.cloudinary.com/dasy5hwz4/image/upload/v1733157906/jwno28yfi8l6cpgzsz8y.jpg",
                "https://res.cloudinary.com/dasy5hwz4/image/upload/v1733157912/sxno17t5bwuwxl7dmqvv.jpg"
                    },
                    CourtDetails = new List<CourtDetailsDto>
            {
                new CourtDetailsDto
                {
                    CourtName = "Sân 44",
                    Status = CourtStatus.Available
                }
            }
                };

                // Act: Gửi command Edit
                var result = await Mediator.Send(new Edit.Command
                {
                    courtCluster = courtClusterInputDto,
                    id = id
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
        int.MaxValue,
        "Sân pickleball",
        "83",
        "Tỉnh Bến Tre",
        "834",
        "Huyện Giồng Trôm",
        "29047",
        "Xã Hưng Lễ",
        "Ngõ 182",
        "b6341ccf-1a22-426c-83bd-21f3f63cd83f",
        ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditCourtCluster_WhenCourtClusterNotFound(
        int id,
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
                    OpenTime = TimeOnly.Parse("05:00:00"),
                    CloseTime = TimeOnly.Parse("22:00:00"),
                    CreatedAt = DateTime.Now,
                    Description = "<p>sân</p>",
                    Images = new string[]
                    {
                "https://res.cloudinary.com/dasy5hwz4/image/upload/v1733157906/jwno28yfi8l6cpgzsz8y.jpg",
                "https://res.cloudinary.com/dasy5hwz4/image/upload/v1733157912/sxno17t5bwuwxl7dmqvv.jpg"
                    },
                    CourtDetails = new List<CourtDetailsDto>
            {
                new CourtDetailsDto
                {
                    CourtName = "Sân 44",
                    Status = CourtStatus.Available
                }
            }
                };

                // Act: Gửi command Edit
                var result = await Mediator.Send(new Edit.Command
                {
                    courtCluster = courtClusterInputDto,
                    id = id
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
    }
}
