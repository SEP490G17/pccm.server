using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Application.Handler.Courts;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Courts
{
    public class DetailCourtHandlerTests
    {
            private readonly IMediator Mediator;

        public DetailCourtHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(4, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailCourt_WhenExistCourt(
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

         [TestCase(130, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDetailCourt_WhenNotExistId(
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