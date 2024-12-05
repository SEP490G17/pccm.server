using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.Courts;


namespace Pccm.UnitTest.Courts
{
    public class ToggleCourtHandlerTests
    {
          private readonly IMediator Mediator;

        public ToggleCourtHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(10,1, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldUpdateCourt(
            int id,
             int Status)
        {
            try
            {
                var result = await Mediator.Send(new ToggleCourt.Command() { Id = id, Status =Status }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase(457,1, ExpectedResult = false)]
        public async Task<bool> Handle_UpdateCourtFail_WhenNotExistCourt(
            int id,
            int Status)
        {
            try
            {
                var result = await Mediator.Send(new ToggleCourt.Command() { Id = id, Status = Status }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
