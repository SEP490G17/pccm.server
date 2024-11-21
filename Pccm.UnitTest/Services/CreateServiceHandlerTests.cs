using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;
using Application.Handler.Services;

namespace Pccm.UnitTest.Services
{
    [TestFixture]
    public class CreateBannerHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateBannerHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(new int[] { 1 }, "Premium Service 2", "High-quality tennis court rental", 150, ExpectedResult = true)]
        public async Task<bool> Handle_CreateService_WhenValid(
            int[] CourtClusterId,
            string ServiceName,
            string Description,
            decimal Price)
        {
            try
            {
                var serviceInputDto = new ServiceInputDto()
                {
                    CourtClusterId = CourtClusterId,
                    ServiceName = ServiceName,
                    Description = Description,
                    Price = Price
                };

                var result = await Mediator.Send(new Create.Command() { Service = serviceInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(new int[] { 1 }, "Premium Service 2", "High-quality tennis court rental", 150, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateServiceFail_WhenExistServiceCourtCluster(
            int[] CourtClusterId,
            string ServiceName,
            string Description,
            decimal Price)
        {
            try
            {
                var serviceInputDto = new ServiceInputDto()
                {
                    CourtClusterId = CourtClusterId,
                    ServiceName = ServiceName,
                    Description = Description,
                    Price = Price
                };

                var result = await Mediator.Send(new Create.Command() { Service = serviceInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
