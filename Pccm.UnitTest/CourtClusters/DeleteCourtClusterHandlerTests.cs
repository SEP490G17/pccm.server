using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.CourtClusters;


namespace Pccm.UnitTest.CourtClusters
{
    public class DeleteCourtClusterHandlerTests
    {
        private readonly IMediator Mediator;

        public DeleteCourtClusterHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(200, ExpectedResult = false)]
        public async Task<bool> Handle_DeleteCourtCluster_WhenNotExistCourtCluster(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Delete.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(9, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDeleteCourtCluster_WhenExistCourtCluster(
           int id)
        {
            try
            {
                var result = await Mediator.Send(new Delete.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
