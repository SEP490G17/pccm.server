using API.Extensions;
using Application.Handler.Banners;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Banners
{
    public class DetailBannerHandlerTests
    {
        private readonly IMediator Mediator;

        public DetailBannerHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(4, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailBanner_WhenValidId(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Details.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(130, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDetailBannerFail_WhenNotExistId(
           int id)
        {
            try
            {
                var result = await Mediator.Send(new Details.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}