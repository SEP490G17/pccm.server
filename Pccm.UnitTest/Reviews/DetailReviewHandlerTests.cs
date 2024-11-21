using API.Extensions;
using Application.Handler.Reviews;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Reviews
{
    public class DetailReviewHandlerTests
    {
        private readonly IMediator Mediator;

        public DetailReviewHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }

        [TestCase(2, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailReview_WhenValidId(
           int id)
        {
           try
           {
               var result = await Mediator.Send(new ListCourtCluster.Query() { Id = id }, default);

               return result.IsSuccess;
           }
           catch (Exception ex)
           {
               return false;
           }
        }
    }
}