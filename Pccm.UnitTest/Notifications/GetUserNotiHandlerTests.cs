using API.Extensions;
using Application.SpecParams;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Security.Claims;
namespace Pccm.UnitTest.Notifications
{
    [TestFixture]
    public class GetUserNotiHandlerTests
    {
              public readonly IMediator Mediator;

        public GetUserNotiHandlerTests()
        {
             var builder = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddApplicationService(context.Configuration);

                // Add IHttpContextAccessor mock
                var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
                var httpContext = new DefaultHttpContext();
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "adminstrator"),
                    new Claim(ClaimTypes.NameIdentifier, "b6341ccf-1a22-426c-83bd-21f3f63cd83f")
                }));
                httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);
                services.AddSingleton<IHttpContextAccessor>(httpContextAccessorMock.Object);
            });

        var host = builder.Build();
        Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(0, 5, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListNewsBlog_WhenValidId(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Notifications.GetUserNoti.Query()
            {
                BaseSpecParam = new BaseSpecWithFilterParam()
                {
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.IsSuccess;
        }

    }
}