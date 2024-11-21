using API.Extensions;
using Application.Handler.Banners;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Banners
{
    public class ChangeStatusBannerHandlerTests
    {
        private readonly IMediator Mediator;

        public ChangeStatusBannerHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(1, 0, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldChangeStatusBanner_WhenValid(
             int id,
             int Status
            )
        {
            try
            {
                var result = await Mediator.Send(new ChangeStatus.Command() { Id = id, status = Status }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(111, 1, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldChangeStatusBannerFail_WhenNotExistBannerID(
             int id,
             int Status
            )
        {
            try
            {
                var result = await Mediator.Send(new ChangeStatus.Command() { Id = id, status = Status }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
