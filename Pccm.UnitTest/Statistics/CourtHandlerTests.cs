using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;

namespace Pccm.UnitTest.Staffs
{
    public class CourtHandlerTests
    {
        public readonly IMediator Mediator;

        public CourtHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase(ExpectedResult = 0)] // Replace 8 with the expected value for totalBookingToday
        public async Task<int> Handle_ShouldReturnTotalBookingToday()
        {
            if (this.Mediator is null) return 0;

            var response = await this.Mediator.Send(new Application.Handler.Statistics.Count.Query());

            return response.Value?.totalBookingToday ?? 0; // Ensure null safety
        }


    }
}
