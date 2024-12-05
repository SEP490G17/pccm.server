using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.Products;
using Application.DTOs;


namespace Pccm.UnitTest.Products
{
    public class ImportProductHandlerTests
    {
        private readonly IMediator Mediator;

        public ImportProductHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(10, 10, 100.50,"adminstrator", ExpectedResult = true)]  // Example values for Quantity and ImportFee
        public async Task<bool> Handle_ShouldImportProduct_WhenValid(
            int id,
            int quantity,
            decimal importFee,
            string userName)
        {
            try
            {
                var productImportDto = new ProductImportDto()
                {
                    Quantity = quantity,
                    ImportFee = importFee
                };

                // Use the new ProductImportDto in the command
                var result = await Mediator.Send(new ImportProduct.Command() { Id = id,userName =userName, product = productImportDto }, default);

                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }


      [TestCase(2004, 10, 100.50,"adminstrator", ExpectedResult = false)]  // Example values for Quantity and ImportFee
        public async Task<bool> Handle_ShouldDeleteProductFail_WhenNotExistProduct(
            int id,
            int quantity,
            decimal importFee,
            string userName)
        {
            try
            {
                var productImportDto = new ProductImportDto()
                {
                    Quantity = quantity,
                    ImportFee = importFee
                };

                // Use the new ProductImportDto in the command
                var result = await Mediator.Send(new ImportProduct.Command() { Id = id,userName =userName, product = productImportDto }, default);

                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
