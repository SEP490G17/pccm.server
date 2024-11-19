using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;
using Application.SpecParams.BookingSpecification;
using Domain.Enum;

namespace Pccm.UnitTest.Bookings
{
    public class ListBookingV1HandlerTests
    {
        public readonly IMediator Mediator;

        public ListBookingV1HandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


       
        [TestCase(0, 5, ExpectedResult = 0)]
        public async Task<int?> Handle_ShouldListBooking_WhenValid(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Bookings.ListV1.Query()
            {
                BookingSpecParam = new BookingV1SpecParam()
                {
                    CourtClusterId = 11
                }
            });

            return response.Value.Count();
        }

         [TestCase(0, 5, ExpectedResult = 0)]
        public async Task<int?> Handle_ShouldListBooking_WhenFilterByBookingStatus(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Bookings.ListV1.Query()
            {
                BookingSpecParam = new BookingV1SpecParam()
                {
                    BookingStatus = BookingStatus.Cancelled,
                      CourtClusterId = 1,
                }
            });

            return response.Value.Count();
        }
        
        
    }
}
