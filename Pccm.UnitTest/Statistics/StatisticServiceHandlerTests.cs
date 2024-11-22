using API.Extensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Statistics
{
    public class StatisticServiceHandlerTests
    {
        public readonly IMediator Mediator;

        public StatisticServiceHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }

        [TestCase("09", "2024", 1, ExpectedResult = 0)]
        public async Task<decimal?> Handle_ShouldShowStatisticService_WhenFilterByMonthAndYear(
            string month,
            string year,
            int courtClusterId)
        {
            if (this.Mediator is null)
            {
                return null;
            }

            var inputDto = new Application.DTOs.StatisticInputDTO
            {
                Month = month,
                Year = year,
                CourtClusterId = courtClusterId
            };

            var response = await this.Mediator.Send(new Application.Handler.Statistics.StatisticService.Query { StatisticInput = inputDto });
            // Ensure response is valid and return TotalAmount or 0 if no results
            var statisticResult = response?.Value?.FirstOrDefault();
            return statisticResult?.TotalAmount;
        }

        [TestCase("0", "2024", 1, ExpectedResult = 0)]
        public async Task<decimal?> Handle_ShouldShowAllStatisticService_WhenValidData(
            string month,
            string year,
            int courtClusterId)
        {
            if (this.Mediator is null)
            {
                return null;
            }

            var inputDto = new Application.DTOs.StatisticInputDTO
            {
                Month = month,
                Year = year,
                CourtClusterId = courtClusterId
            };

            var response = await this.Mediator.Send(new Application.Handler.Statistics.StatisticService.Query { StatisticInput = inputDto });
            // Ensure response is valid and return TotalAmount or 0 if no results
            var statisticResult = response?.Value?.FirstOrDefault();
            return statisticResult?.TotalAmount;
        }

    }
}