using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.Products;


namespace Pccm.UnitTest.Products
{
    public class DeleteProductHandlerTests
    {
          private readonly IMediator Mediator;

        public DeleteProductHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(200, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDeleteProductFail_WhenNotExistProduct(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Delete.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase(10, ExpectedResult = true)]
        public async Task<bool> Handle_DeleteProduct_WhenExistProduct(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Delete.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
