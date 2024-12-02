using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams.BookingSpecification;

namespace Pccm.UnitTest.Bookings
{
    public class ListBookingV2HandlerTests
    {
        public readonly IMediator Mediator;

        public ListBookingV2HandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


       
        [TestCase(0, 5, ExpectedResult = 5)]
        public async Task<int?> Handle_ShouldListBooking_WhenValid(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Bookings.ListV2.Query()
            {
                BookingSpecParam = new BookingSpecParam()
                {
                    Search = "",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }

         [TestCase(0, 5, ExpectedResult = 2)]
        public async Task<int?> Handle_ShouldListBooking_WhenFilterByCourtCluster(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Bookings.ListV2.Query()
            {
                BookingSpecParam = new BookingSpecParam()
                {
                    Search = "",
                    Skip = 0,
                    CourtClusterId = 2,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }
        
        
    }
}
