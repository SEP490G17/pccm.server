using API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Statistics
{
    public class ExportExcelHandlerTests
    {
        public readonly IMediator Mediator;

        public ExportExcelHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("2024-09-09", 1, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldExportExcel_WhenValidData(
            DateTime dateTime,
            int courtClusterId
        )
        {
            if (this.Mediator is null)
            {
                return null; 
            }

            var response = await this.Mediator.Send(new Application.Handler.Statistics.ExportExcel.Query(){Date = dateTime, CourtClusterId = courtClusterId});

                 return response.IsSuccess && response.Value is FileContentResult fileResult &&
           fileResult.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
    }
}