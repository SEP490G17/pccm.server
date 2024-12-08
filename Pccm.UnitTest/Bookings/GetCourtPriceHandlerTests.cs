using Application.Handler.Bookings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class GetCourtPriceHandlerTests
    {
        private readonly IMediator Mediator;

        public GetCourtPriceHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(3, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCourtPrice_WhenValidData(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new GetCourtPrice.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

      [TestCase(1111, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDeclineBookingFail_WhenNotExistCourtID(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new GetCourtPrice.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       
    }
}
