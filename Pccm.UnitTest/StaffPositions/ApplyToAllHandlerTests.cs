using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;

namespace Pccm.UnitTest.Services
{
    public class ApplyToAllHandlerTests
    {
        public readonly IMediator Mediator;

        public ApplyToAllHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase(0, 5, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldApllyToAll_WhenValid(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.StaffPositions.ApplyToAll.Command()
            {
            });

            return response.IsSuccess;
        }

         [TestCase(0, 5, ExpectedResult = 5)]
        public async Task<int?> Handle_ShouldListService_WhenFilterByCourtClusterID(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Services.List.Query()
            {
                BaseSpecParam = new BaseSpecWithFilterParam()
                {
                    Search = "",
                    Skip = 0,
                    Filter = 2,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }
    }
}
