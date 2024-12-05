using Application.Handler.Bookings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class GetSlotHandlerTests
    {
        private readonly IMediator Mediator;

        public GetSlotHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("2024-11-29",4, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldAvailableSlot_WhenValidData(
            string Date,
            int CourtClusterId)
        {
            try
            {
                var result = await Mediator.Send(new GetSlot.Query() { Date = Date, CourtClusterId =  CourtClusterId}, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       [TestCase("2024-1129",4, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldAvailableSlotFail_WhenInvalidDate(
               string Date,
            int CourtClusterId)
        {
            try
            {
                var result = await Mediator.Send(new GetSlot.Query() { Date = Date, CourtClusterId =  CourtClusterId}, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase("2024-11-29",421, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldAvailableSlotFail_WhenCourtNotExist(
               string Date,
            int CourtClusterId)
        {
            try
            {
                var result = await Mediator.Send(new GetSlot.Query() { Date = Date, CourtClusterId =  CourtClusterId}, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }
}
