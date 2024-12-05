using API.Extensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Statistics
{
    public class TopStatisticsServiceHandlerTests
    {
        public readonly IMediator Mediator;

        public TopStatisticsServiceHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("12", "2024", ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldShowTopStatisticCourtCluster_WhenValidData(
            string month,
            string year
        )
        {
            if (this.Mediator is null)
            {
                return null;
            }

            var response = await this.Mediator.Send(new Application.Handler.Statistics.TopStatisticsService.Query() { Month = month, Year = year });

            // Check if the response is successful
            if (!response.IsSuccess)
            {
                return false;
            }

            // Validate the response data
            var topStatistic = response.Value;

            if (topStatistic is null ||
                topStatistic.TopStaffs == null || !topStatistic.TopStaffs.Any() ||
                topStatistic.TopProducts == null || !topStatistic.TopProducts.Any())
            {
                return false; // Ensure non-null and contains data
            }

            // Optionally check specific details of the result (counts, expected data, etc.)
            return true;
        }
    }
}