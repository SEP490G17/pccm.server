using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;
using Application.Handler.CourtClusters;

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


        [TestCase("Cụm sân A", "HCM", "TP Hồ Chí Minh", "Q1", "Quận 1", "Phường 1", "Phuong 1", "Ha Nam", "612c71e2-a43e-426f-86c6-75820d61b36f", ExpectedResult = true)]
        public async Task<bool> Handle_CreateCourtCluster_WhenValid(
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



        [TestCase("Cụm sân A", "HCM", "TP Hồ Chí Minh", "Q1", "Quận 1", "Phường 1", "Phuong 1", "Ha Nam", "612c71e2-a43e-426f-86c6-75820d61b36ftt", ExpectedResult = false)]
        public async Task<bool> Handle_CreateCourtCluster_WhenNotExistOwnerId(
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

        [TestCase(null, "HCM", "TP Hồ Chí Minh", "Q1", "Quận 1", "Phường 1", "Phuong 1", "Ha Nam", "612c71e2-a43e-426f-86c6-75820d61b36f", ExpectedResult = false)]
        public async Task<bool> Handle_CreateCourtCluster_WhenTitleIsNull(
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
