using API.Extensions;
using Application.Handler.CourtClusters;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.CourtClusters
{
    public class DetailUserSiteHandlerTests
    {
        private readonly IMediator Mediator;

        public DetailUserSiteHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(5, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailCourtCluster_WhenExistCourtCluster(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new DetailUserSite.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(int.MaxValue, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDetailCourtClusterFail_WhenNotExistId(
           int id)
        {
            try
            {
                var result = await Mediator.Send(new DetailUserSite.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}