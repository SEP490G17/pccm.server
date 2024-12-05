using API.Extensions;
using Application.Handler.News;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.News
{
    public class ChangeStatusNewsHandlerTests
    {
         private readonly IMediator Mediator;

        public ChangeStatusNewsHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(4, 1, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldEditNewsBlog_WhenValidId(
            int id,
            int status)
        {
            try
            {
                var result = await Mediator.Send(new ChangeStatus.Command() { Id = id, status =status }, default);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
        [TestCase(5, 0, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldEditNewsBlog_WhenValidIdWithStatusDisplay(
            int id,
            int status)
        {
            try
            {
                var result = await Mediator.Send(new ChangeStatus.Command() { Id = id, status =status }, default);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}