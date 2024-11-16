using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;

namespace Pccm.UnitTest.Services
{
    public class ListBannerHandlerTests
    {
        public readonly IMediator Mediator;

        public ListBannerHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase(0, 5, ExpectedResult = 1)]
        public async Task<int?> Handle_ShouldListService_WhenValid(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Services.List.Query()
            {
                BaseSpecParam = new BaseSpecWithFilterParam()
                {
                    Search = "",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }

        [TestCase(0, 5, ExpectedResult = 1)]
        public async Task<int?> Handle_ShouldListService_WhenSearchByName(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Services.List.Query()
            {
                BaseSpecParam = new BaseSpecWithFilterParam()
                {
                    Search = "Cho thuê vợt pickleball",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }
    }
}
