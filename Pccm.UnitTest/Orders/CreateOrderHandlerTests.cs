using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;
using Application.Handler.Orders;

namespace Pccm.UnitTest.Orders
{
    [TestFixture]
    public class CreateOrderHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateOrderHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(12, ExpectedResult = true)]
        public async Task<bool> Handle_CreateOrder_WhenValid(
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
                var result = await Mediator.Send(new OrderCreateV1.Command()
                {
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



        [TestCase(101, ExpectedResult = false)]
        public async Task<bool> Handle_CreateOrder_WhenIDBookingNotExist(
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
                var result = await Mediator.Send(new OrderCreateV1.Command()
                {
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
