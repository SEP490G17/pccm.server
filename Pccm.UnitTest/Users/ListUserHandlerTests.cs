using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;

namespace Pccm.UnitTest.Users
{
    public class ListUserHandlerTests
    {
        public readonly IMediator Mediator;

        public ListUserHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase(0, 5, ExpectedResult = 5)]
        public async Task<int?> Handle_ShouldListUser(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Users.List.Query()
            {
                BaseSpecParam = new BaseSpecParam()
                {
                    Search = "",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }

        [TestCase(0, 5, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListUser_WhenSearch(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Users.List.Query()
            {
                BaseSpecParam = new BaseSpecParam()
                {
                    Search = "Alexandros",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.IsSuccess;
        }

    }
}
