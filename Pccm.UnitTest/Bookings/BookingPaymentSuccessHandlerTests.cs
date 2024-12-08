using Application.Handler.Bookings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class BookingPaymentSuccessHandlerTests
    {
        private readonly IMediator Mediator;

        public BookingPaymentSuccessHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(22, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldBookingPaymentSuccess_WhenValidData(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new BookingPaymentSuccess.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(111, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldBookingPaymentFail_WhenNotExistBooking(
              int id)
        {
            try
            {
                var result = await Mediator.Send(new BookingPaymentSuccess.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
