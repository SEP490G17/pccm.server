using API.Extensions;
using Application.Handler.Users;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Users
{
    public class DetailUserHandlerTests
    {
        private readonly IMediator Mediator;

        public DetailUserHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("staff10", ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailUser_WhenExist(
            string username)
        {
            try
            {
                var result = await Mediator.Send(new Detail.Query() { username = username }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("Thanhhung", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDetailUser_WhenNotExistUser(
           string username)
        {
            try
            {
                var result = await Mediator.Send(new Detail.Query() { username = username }, default);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}