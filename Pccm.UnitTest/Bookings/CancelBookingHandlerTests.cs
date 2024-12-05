using Application.Handler.Bookings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class CancelBookingHandlerTests
    {
        private readonly IMediator Mediator;

        public CancelBookingHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(13, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCancelBooking_WhenValidData(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

      [TestCase(111, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCancelBookingFail_WhenNotExistBooking(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase(3, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCancelBookingFail_WhenBookingIsSuccess(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
       
    }
}
