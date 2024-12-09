using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams.ProductSpecification;

namespace Pccm.UnitTest.Banners
{
    public class BannerValidatorHandlerTests
    {
        public readonly IMediator Mediator;

        public BannerValidatorHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase( ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListBanner_WhenValid()
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Banners.ListUserSite.Query()
            {
            });
 
            return response.IsSuccess;
        }

    }
}