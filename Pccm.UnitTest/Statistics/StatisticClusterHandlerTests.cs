using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Statistics
{
    public class StatisticClusterHandlerTests
    {
        public readonly IMediator Mediator;

        public StatisticClusterHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("2024-09-09", 1, ExpectedResult = 0)]
        public async Task<int?> Handle_ShouldShowStatisticCourtCluster_WhenValidData(
            DateTime dateTime,
            int courtClusterId
        )
        {
            if (this.Mediator is null)
            {
                return null; 
            }

            var response = await this.Mediator.Send(new Application.Handler.Statistics.StatisticCluster.Query(){Date = dateTime, CourtClusterId = courtClusterId});

               return response?.Value?.BookingDetails?.Count();
        }
    }
}