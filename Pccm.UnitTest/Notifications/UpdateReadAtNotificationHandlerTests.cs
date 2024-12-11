using API.Extensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Pccm.UnitTest.Notifications
{
    [TestFixture]
    public class UpdateReadAtNotificationHandlerTests
    {
        public readonly IMediator Mediator;

        public UpdateReadAtNotificationHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(0, 5,3, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldUpdateNotification_WhenValidId(int skip, int pageSize, int id)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Notifications.UpdateReadAtNotification.Command()
            {
                NotiId = id
            });

            return response.IsSuccess;
        }

    }
}