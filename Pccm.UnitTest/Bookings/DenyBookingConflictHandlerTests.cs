using Application.Handler.Bookings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class DenyBookingConflictHandlerTests
    {
        private readonly IMediator Mediator;

        public DenyBookingConflictHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(new[] { 14 }, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDeclineBooking_WhenValidData(
            int[] ids)
        {
            try
            {
                 var idList = ids.ToList(); 
                var result = await Mediator.Send(new DenyBookingConflict.Command() { Id = idList }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

      [TestCase(new[] { 133 }, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDeclineBookingFail_WhenIDNotExist(
            int[] ids)
        {
            try
            {
                 var idList = ids.ToList(); 
                var result = await Mediator.Send(new DenyBookingConflict.Command() { Id = idList }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase(new[] { 14 }, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDeclineBookingFail_WhenBookingIsSuccess(
            int[] ids)
        {
            try
            {
                 var idList = ids.ToList(); 
                var result = await Mediator.Send(new DenyBookingConflict.Command() { Id = idList }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       
    }
}
