using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;

namespace Pccm.UnitTest.Statistics
{
    public class GetYearsHandlerTests
    {
        public readonly IMediator Mediator;

        public GetYearsHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListYear_WhenValid()
        {
            if (this.Mediator is null)
            {
                return null; 
            }

            var response = await this.Mediator.Send(new Application.Handler.Statistics.GetYears.Query());

            return response.IsSuccess;
        }
    }
}
