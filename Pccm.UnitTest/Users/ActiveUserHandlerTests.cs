using API.Extensions;
using Application.DTOs;
using Application.Handler.Users;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Users
{
    public class ActiveUserHandlerTests
    {
        private readonly IMediator Mediator;

        public ActiveUserHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("staff10", true, ExpectedResult = true)]
        public async Task<bool> Handle_ActiveUser_WhenValid(
         string username,
         bool isActive)
        {
            try
            {
                var activeDto = new ActiveDto()
                {
                    username = username,
                    IsActive = isActive
                };

                var result = await Mediator.Send(new ActiveUser.Command() { user = activeDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("Thanhhung", true, ExpectedResult = false)]
        public async Task<bool> Handle_ActiveUser_WhenNotExistUserID(
        string username,
        bool isActive)
        {
            try
            {
                var activeDto = new ActiveDto()
                {
                    username = username,
                    IsActive = isActive
                };

                var result = await Mediator.Send(new ActiveUser.Command() { user = activeDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
