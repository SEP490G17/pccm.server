using Application.Handler.Bookings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class BookingDetailsV1HandlerTests
    {
        private readonly IMediator Mediator;

        public BookingDetailsV1HandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(13, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldBookingDetails_WhenValidData(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new BookingDetailsV1.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(111, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldBookingDetailsFail_WhenNotExistBooking(
              int id)
        {
            try
            {
                var result = await Mediator.Send(new BookingDetailsV1.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
