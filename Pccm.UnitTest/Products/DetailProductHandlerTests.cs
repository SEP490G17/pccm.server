using API.Extensions;
using Application.Handler.Products;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Products
{
    public class DetailProductHandlerTests
    {
            private readonly IMediator Mediator;

        public DetailProductHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(4, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailProduct_WhenExistProduct(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Details.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
    }
}