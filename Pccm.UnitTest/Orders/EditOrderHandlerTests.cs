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


        [TestCase(7, 4, 1, "2024-11-11T10:00:00", "2024-11-11T12:00:00", 1000.00, "Pending", ExpectedResult = true)]
        public async Task<bool> Handle_EditOrder_WhenValid(
            int id,
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
                    Id = id,
                    BookingId = BookingId,
                    StaffId = StaffId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    TotalAmount = TotalAmount,
                    Status = Status
                };

                var result = await Mediator.Send(new Edit.Command() { order = orderInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(7, 114, 1, "2024-11-11T10:00:00", "2024-11-11T12:00:00", 1000.00, "Pending", ExpectedResult = false)]
        public async Task<bool> Handle_EditOrderFail_WhenBookingIdNotExist(
           int id,
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
                    Id = id,
                    BookingId = BookingId,
                    StaffId = StaffId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    TotalAmount = TotalAmount,
                    Status = Status
                };

                var result = await Mediator.Send(new Edit.Command() { order = orderInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(7, 4, 111, "2024-11-11T10:00:00", "2024-11-11T12:00:00", 1000.00, "Pending", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditOrderFail_WhenStaffIdNotExist(
          int id,
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
                    Id = id,
                    BookingId = BookingId,
                    StaffId = StaffId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    TotalAmount = TotalAmount,
                    Status = Status
                };

                var result = await Mediator.Send(new Edit.Command() { order = orderInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
