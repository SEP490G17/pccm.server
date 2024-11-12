using MediatR;
using NUnit.Framework;
using Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entity;
using Moq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.Orders;

namespace Pccm.UnitTest.Orders
{
    public class DeleteOrderHandlerTests
    {
          private readonly IMediator Mediator;

        public DeleteOrderHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(7, ExpectedResult = true)]
        public async Task<bool> Handle_DeleteOrder_WhenValid(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Delete.Command() { Id = id }, default);

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
