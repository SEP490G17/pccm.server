using API.Extensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.News
{
    public class ListNewsUserSiteHandlerTests
    {
        public readonly IMediator Mediator;

        public ListNewsUserSiteHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListNewsUserSiteBlog_WhenValidId()
        {
            if (this.Mediator is null)
            {
                return null; 
            }

            var response = await this.Mediator.Send(new Application.Handler.News.ListNewsUserSite.Query());

            return response.IsSuccess;
        }
    }
}