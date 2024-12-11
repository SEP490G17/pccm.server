using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;
using Application.Handler.Statistics;

namespace Pccm.UnitTest.Reviews
{
    [TestFixture]
    public class SaveRevenueHandlerTests
    {
        private readonly IMediator Mediator;

        public SaveRevenueHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("b6341ccf-1a22-426c-83bd-21f3f63cd83f", 4, "2024-11-11", ExpectedResult = true)]
        public async Task<bool> Handle_ShouldSaveRevenue_WhenValid(
            string UserId,
            int courtClusterId,
            string Date)
        {
            try
            {
                var saveRevenueDto = new SaveRevenueDto()
                {
                    Date = DateTime.Parse(Date),
                    courtClusterId = courtClusterId,
                    OrderProductDetails = new List<OrderProductDetailDto>
            {
                new OrderProductDetailDto
                {
                    // Populate with appropriate properties
                    // Example:
                    ProductName = "Product A",
                    Quantity = 10,
                    TotalPrice = 200
                }
            },
                    BookingDetails = new List<BookingDetailDto>
            {
                new BookingDetailDto
                {
                    CourtName = "San 1",
                    HoursBooked = "12",
                    TotalPrice = 150
                   
                }
            },
                    OrderServiceDetails = new List<OrderServiceDetailDto>
            {
                new OrderServiceDetailDto
                {
                    // Populate with appropriate properties
                    // Example:
                    ServiceName = "Service B",
                    Quantity = 2,
                    TotalPrice = 300
                }
            },
                    ExpenseDetails = new List<ExpenseDetailDto>
            {
                new ExpenseDetailDto
                {
                    // Populate with appropriate properties
                    // Example:
                    ExpenseName = "Tien điện",
                    TotalPrice = 1000
                }
            }
                };

                var result = await Mediator.Send(new SaveRevenue.Command() { revenue = saveRevenueDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
    }
}
