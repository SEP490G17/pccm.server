using API.Extensions;
using Application.Handler.Orders;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Orders
{
    public class DetailV1OrderHandlerTests
    {
        private readonly IMediator Mediator;

        public DetailV1OrderHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(8, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailOrder_WhenValidId(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new DetailV1.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(117, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDetailOrderFail_WhenIdNotExist(
           int id)
        {
            try
            {
                var result = await Mediator.Send(new DetailV1.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}