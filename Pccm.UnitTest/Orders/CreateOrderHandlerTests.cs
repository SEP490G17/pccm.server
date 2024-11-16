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


        [TestCase(4, 1, "2024-11-11T10:00:00", "2024-11-11T12:00:00", 1000.00, "Pending", ExpectedResult = true)]
        public async Task<bool> Handle_CreateOrder_WhenValid(
            int BookingId,
            int StaffId,
            string StartTime,
            string EndTime,
            decimal TotalAmount,
            string Status)
        {
            try
            {
                var orderInputDto = new OrderInputDto()
                {
                    BookingId = BookingId,
                    StaffId = StaffId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    TotalAmount = TotalAmount,
                    Status = Status
                };

                var result = await Mediator.Send(new Create.Command() { order = orderInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(114, 1, "2024-11-11T10:00:00", "2024-11-11T12:00:00", 1000.00, "Pending", ExpectedResult = false)]
        public async Task<bool> Handle_CreateOrder_WhenBookingIdNotExist(
          int BookingId,
          int StaffId,
          string StartTime,
          string EndTime,
          decimal TotalAmount,
          string Status)
        {
            try
            {
                var orderInputDto = new OrderInputDto()
                {
                    BookingId = BookingId,
                    StaffId = StaffId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    TotalAmount = TotalAmount,
                    Status = Status
                };

                var result = await Mediator.Send(new Create.Command() { order = orderInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(4, 111, "2024-11-11T10:00:00", "2024-11-11T12:00:00", 1000.00, "Pending", ExpectedResult = false)]
        public async Task<bool> Handle_CreateOrder_WhenStaffIdNotExist(
          int BookingId,
          int StaffId,
          string StartTime,
          string EndTime,
          decimal TotalAmount,
          string Status)
        {
            try
            {
                var orderInputDto = new OrderInputDto()
                {
                    BookingId = BookingId,
                    StaffId = StaffId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    TotalAmount = TotalAmount,
                    Status = Status
                };

                var result = await Mediator.Send(new Create.Command() { order = orderInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
