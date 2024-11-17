using API.Extensions;
using Application.Handler.Categories;
using Domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Categories
{
    public class EditCategoryHandlerTests
    {
        private readonly IMediator Mediator;

        public EditCategoryHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(2, "Đồ uống ", ExpectedResult = true)]
        public async Task<bool> Handle_ShouldEditCategory_WhenValidData(
           int id,
         string CategoryName)
        {
            try
            {
                var category = new Category
                {
                    Id = id,
                    CategoryName = CategoryName
                };

                var result = await Mediator.Send(new Edit.Command() { Category = category }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [TestCase(2, null, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditCategoryFail_WhenCategoryNameIsNull(
          int id,
        string? CategoryName)
        {
            try
            {
                var category = new Category
                {
                    Id = id,
                    CategoryName = CategoryName
                };

                var result = await Mediator.Send(new Edit.Command() { Category = category }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
