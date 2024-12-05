using API.Extensions;
using Application.DTOs;
using Application.Handler.Orders;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Orders
{
    public class EditOrderHandlerTests
    {
        private readonly IMediator Mediator;

        public EditOrderHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


       [TestCase(94, 10, ExpectedResult = true)]
        public async Task<bool> Handle_EditOrder_WhenValid(
            int Id ,
         int bookingId)
        {
            try
            {
                var orderForProductsList = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto
                    {
                        ProductId = 8,
                        Quantity = 1
                    }
                };

                // Giả sử không có dịch vụ, khởi tạo danh sách rỗng cho orderForServices
                var orderForServices = new List<ServicesForOrderCreateDto>();

                // Gửi command để tạo order
                var result = await Mediator.Send(new OrderEditV1.Command()
                {
                    Id = Id,
                    BookingId = bookingId,
                    OrderForProducts = orderForProductsList,
                    OrderForServices = orderForServices
                }, default);

                // Kiểm tra kết quả trả về
                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [TestCase(944, 10, ExpectedResult = false)]
        public async Task<bool> Handle_EditOrder_WhenOrderNotExist(
            int Id ,
         int bookingId)
        {
            try
            {
                var orderForProductsList = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto
                    {
                        ProductId = 8,
                        Quantity = 1
                    }
                };

                // Giả sử không có dịch vụ, khởi tạo danh sách rỗng cho orderForServices
                var orderForServices = new List<ServicesForOrderCreateDto>();

                // Gửi command để tạo order
                var result = await Mediator.Send(new OrderEditV1.Command()
                {
                    Id = Id,
                    BookingId = bookingId,
                    OrderForProducts = orderForProductsList,
                    OrderForServices = orderForServices
                }, default);

                // Kiểm tra kết quả trả về
                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }

       [TestCase(94, 10, ExpectedResult = false)]
        public async Task<bool> Handle_EditOrderFail_WhenListProductIsEmpt(
            int Id ,
         int bookingId)
        {
            try
            {
                var orderForProductsList = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto
                    {
                    }
                };

                // Giả sử không có dịch vụ, khởi tạo danh sách rỗng cho orderForServices
                var orderForServices = new List<ServicesForOrderCreateDto>();

                // Gửi command để tạo order
                var result = await Mediator.Send(new OrderEditV1.Command()
                {
                    Id = Id,
                    BookingId = bookingId,
                    OrderForProducts = orderForProductsList,
                    OrderForServices = orderForServices
                }, default);

                // Kiểm tra kết quả trả về
                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [TestCase(94, 10, ExpectedResult = false)]
        public async Task<bool> Handle_EditOrderFail_WhenProductNotExist(
            int Id ,
         int bookingId)
        {
            try
            {
                var orderForProductsList = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto
                    {
                        ProductId = 811,
                        Quantity = 1
                    }
                };

                // Giả sử không có dịch vụ, khởi tạo danh sách rỗng cho orderForServices
                var orderForServices = new List<ServicesForOrderCreateDto>();

                // Gửi command để tạo order
                var result = await Mediator.Send(new OrderEditV1.Command()
                {
                    Id = Id,
                    BookingId = bookingId,
                    OrderForProducts = orderForProductsList,
                    OrderForServices = orderForServices
                }, default);

                // Kiểm tra kết quả trả về
                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }


         [TestCase(14, 10, ExpectedResult = false)]
        public async Task<bool> Handle_EditOrderFail_WhenInValidProductQuantity(
            int Id ,
         int bookingId)
        {
            try
            {
                var orderForProductsList = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto
                    {
                        ProductId = 8,
                        Quantity = 1
                    }
                };

                // Giả sử không có dịch vụ, khởi tạo danh sách rỗng cho orderForServices
                var orderForServices = new List<ServicesForOrderCreateDto>();

                // Gửi command để tạo order
                var result = await Mediator.Send(new OrderEditV1.Command()
                {
                    Id = Id,
                    BookingId = bookingId,
                    OrderForProducts = orderForProductsList,
                    OrderForServices = orderForServices
                }, default);

                // Kiểm tra kết quả trả về
                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
