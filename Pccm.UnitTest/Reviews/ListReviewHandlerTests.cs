using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;

namespace Pccm.UnitTest.Reviews
{
    public class ListReviewHandlerTests
    {
        public readonly IMediator Mediator;

        public ListReviewHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(ExpectedResult = 6)]
        public async Task<int?> Handle_ShouldListReview_WhenValid()
        {
            if (this.Mediator is null)
            {
                return null; 
            }

            var response = await this.Mediator.Send(new Application.Handler.Reviews.List.Query());

            return response.Value.Count();
        }
    }
}
