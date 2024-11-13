using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Application.Handler.Orders;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Orders
{
    public class DetailOrderHandlerTests
    {
        private readonly IMediator Mediator;

        public DetailOrderHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(7, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailOrder_WhenValidId(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Detail.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(117, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDetailOrder_WhenIdNotExist(
           int id)
        {
            try
            {
                var result = await Mediator.Send(new Detail.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}