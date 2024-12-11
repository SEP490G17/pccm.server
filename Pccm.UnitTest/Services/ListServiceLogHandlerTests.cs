using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams.ProductSpecification;

namespace Pccm.UnitTest.Services
{
    public class ListServiceLogHandlerTests
    {
        public readonly IMediator Mediator;

        public ListServiceLogHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase(0, 5, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListService_WhenValid(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Services.ListServiceLog.Query()
            {
                BaseSpecParam = new ServiceLogSpecParams()
                {
                    Search = "",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.IsSuccess;
        }
       
    }
}
