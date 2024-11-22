using API.Extensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Statistics
{
    public class StatisticExpendHandlerTests
    {
        public readonly IMediator Mediator;

        public StatisticExpendHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("09", "2024", ExpectedResult = 0)]
        public async Task<decimal?> Handle_ShouldShowStatisticExpend_WhenValidData(
            string month,
            string year
        )
        {
            if (this.Mediator is null)
            {
                return null; 
            }

            var response = await this.Mediator.Send(new Application.Handler.Statistics.StatisticExpend.Query(){Month = month, Year = year});

             return response?.Value?.TotalProductExpenditure;
        }
    }
}