using API.Extensions;
using Application.DTOs;
using Application.Handler.Products;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Products
{
    public class EditProductHandlerTests
    {
        private readonly IMediator Mediator;

        public EditProductHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(1, 1, 1, "Premium Product", "High-quality tennis balls", 100, 50.5, 30.0, "url-to-thumbnail", ExpectedResult = true)]
        public async Task<bool> Handle_EditProduct_WhenValid(
             int id,
         int CategoryId,
         int CourtClusterId,
         string ProductName,
         string Description,
         int Quantity,
         decimal PriceSell,
         decimal ImportFee,
         string ThumbnailUrl)
        {
            try
            {
                var productInputDto = new ProductInputDto
                {
                    CategoryId = CategoryId,
                    CourtClusterId = CourtClusterId,
                    ProductName = ProductName,
                    Description = Description,
                    Quantity = Quantity,
                    Price = PriceSell,
                    ImportFee = ImportFee,
                    ThumbnailUrl = ThumbnailUrl
                };

                var result = await Mediator.Send(new Edit.Command { product = productInputDto, Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase(1111, 1, 1, "Premium Product", "High-quality tennis balls", 100, 50.5, 30.0, "url-to-thumbnail", ExpectedResult = false)]
        public async Task<bool> Handle_EditProductFail_WhenNotExistProduct(
             int id,
         int CategoryId,
         int CourtClusterId,
         string ProductName,
         string Description,
         int Quantity,
         decimal PriceSell,
         decimal ImportFee,
         string ThumbnailUrl)
        {
            try
            {
                var productInputDto = new ProductInputDto
                {
                    CategoryId = CategoryId,
                    CourtClusterId = CourtClusterId,
                    ProductName = ProductName,
                    Description = Description,
                    Quantity = Quantity,
                    Price = PriceSell,
                    ImportFee = ImportFee,
                    ThumbnailUrl = ThumbnailUrl
                };

                var result = await Mediator.Send(new Edit.Command { product = productInputDto, Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [TestCase(1, 100, 1, "Premium Product", "High-quality tennis balls", 100, 50.5, 30.0, "url-to-thumbnail", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditProductFail_WhenNotExistCategory(
            int id,
            int CategoryId,
            int CourtClusterId,
            string ProductName,
            string Description,
            int Quantity,
            decimal PriceSell,
            decimal ImportFee,
            string ThumbnailUrl)
        {
            try
            {
                var productInputDto = new ProductInputDto
                {
                    CategoryId = CategoryId,
                    CourtClusterId = CourtClusterId,
                    ProductName = ProductName,
                    Description = Description,
                    Quantity = Quantity,
                    Price = PriceSell,
                    ImportFee = ImportFee,
                    ThumbnailUrl = ThumbnailUrl
                };

                var result = await Mediator.Send(new Edit.Command { product = productInputDto, Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(1, 1, 100, "Premium Product", "High-quality tennis balls", 100, 50.5, 30.0, "url-to-thumbnail", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditProductFail_WhenNotExistCourtCluster(
           int id,
           int CategoryId,
           int CourtClusterId,
           string ProductName,
           string Description,
           int Quantity,
           decimal PriceSell,
           decimal ImportFee,
           string ThumbnailUrl)
        {
            try
            {
                var productInputDto = new ProductInputDto
                {
                    CategoryId = CategoryId,
                    CourtClusterId = CourtClusterId,
                    ProductName = ProductName,
                    Description = Description,
                    Quantity = Quantity,
                    Price = PriceSell,
                    ImportFee = ImportFee,
                    ThumbnailUrl = ThumbnailUrl
                };

                var result = await Mediator.Send(new Edit.Command { product = productInputDto, Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
