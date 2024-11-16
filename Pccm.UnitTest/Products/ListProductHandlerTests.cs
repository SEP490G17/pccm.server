using Application.SpecParams.ProductSpecification;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;

namespace Pccm.UnitTest.Products
{
    public class ListProductHandlerTests
    {
        public readonly IMediator Mediator;

        public ListProductHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


       
        [TestCase(0, 5, ExpectedResult = 5)]
        public async Task<int?> Handle_ShouldListProduct_WhenValid(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Products.List.Query()
            {
                SpecParam = new ProductSpecParams()
                {
                    Search = "",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }
    }
}
