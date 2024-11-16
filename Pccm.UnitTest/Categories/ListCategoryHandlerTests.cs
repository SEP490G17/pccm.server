using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;

namespace Pccm.UnitTest.Categories
{
    public class ListCategoryHandlerTests
    {
        public readonly IMediator Mediator;

        public ListCategoryHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(ExpectedResult = 4)]
        public async Task<int?> Handle_ShouldListCategories_WhenValid()
        {
            if (this.Mediator is null)
            {
                return null;
            }

            var response = await this.Mediator.Send(new Application.Handler.Categories.List.Query());

            return response.Value.Count();
        }
    }
}
