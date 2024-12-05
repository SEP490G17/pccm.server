using Application.Handler.Bookings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class CompletedBookingHandlerTests
    {
        private readonly IMediator Mediator;

        public CompletedBookingHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(14, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCompletedBooking_WhenValidData(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CompletedBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(111, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCompleteBookingFail_WhenNotExistBooking(
              int id)
        {
            try
            {
                var result = await Mediator.Send(new CompletedBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(14, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCancelBookingFail_WhenBookingNotPayment(
           int id)
        {
            try
            {
                var result = await Mediator.Send(new CompletedBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(14, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCancelBookingFail_WhenOrderNotPayment(
          int id)
        {
            try
            {
                var result = await Mediator.Send(new CompletedBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
