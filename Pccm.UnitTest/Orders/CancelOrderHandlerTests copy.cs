using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.Orders;

namespace Pccm.UnitTest.Orders
{
    public class CancelOrderHandlerTests
    {
        private readonly IMediator Mediator;

        public CancelOrderHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(8, ExpectedResult = true)]
        public async Task<bool> Handle_CancelOrder_WhenValid(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelOrderV1.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                // Log or inspect the exception as needed
                return false;
            }
        }

        [TestCase(244, ExpectedResult = false)]
        public async Task<bool> Handle_CancelOrderFail_WhenOrderNotExist(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelOrderV1.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                // Log or inspect the exception as needed
                return false;
            }
        }

        [TestCase(23, ExpectedResult = false)]
        public async Task<bool> Handle_CancelOrderFail_WhenOrderPaymentSuccessfully(
           int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelOrderV1.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                // Log or inspect the exception as needed
                return false;
            }
        }
        [TestCase(93, ExpectedResult = false)]
        public async Task<bool> Handle_CancelOrderFail_WhenOrderCanceled(
          int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelOrderV1.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                // Log or inspect the exception as needed
                return false;
            }
        }
    }
}
