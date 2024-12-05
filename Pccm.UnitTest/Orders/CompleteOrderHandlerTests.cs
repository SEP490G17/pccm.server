using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.Orders;

namespace Pccm.UnitTest.Orders
{
    public class CompleteOrderHandlerTests
    {
        private readonly IMediator Mediator;

        public CompleteOrderHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(8, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCompleteOrder_WhenValid(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CompleteOrder.Command() { Id = id }, default);

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
                var result = await Mediator.Send(new CompleteOrder.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                // Log or inspect the exception as needed
                return false;
            }
        }

         [TestCase(90, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCompleteOrder_WhenOrderPaymentNotExist(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CompleteOrder.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                // Log or inspect the exception as needed
                return false;
            }
        }
          [TestCase(93, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCompleteOrder_WhenOrderPaymentExist(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CompleteOrder.Command() { Id = id }, default);

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
