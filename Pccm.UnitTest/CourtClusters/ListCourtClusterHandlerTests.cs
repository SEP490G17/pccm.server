using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;

namespace Pccm.UnitTest.CourtClusters
{
    public class ListCourtClusterHandlerTests
    {
        public readonly IMediator Mediator;

        public ListCourtClusterHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase(0, 5, ExpectedResult = 5)]
        public async Task<int?> Handle_ShouldListCoutCluster_WhenValid(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.CourtClusters.List.Query()
            {
                BaseSpecWithFilterParam = new BaseSpecWithFilterParam()
                {
                    Search = "",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }

        [TestCase(0, 5, ExpectedResult = 1)]
        public async Task<int?> Handle_ShouldListCoutCluster_WhenSearchByCourtClusterName(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.CourtClusters.List.Query()
            {
                BaseSpecWithFilterParam = new BaseSpecWithFilterParam()
                {
                    Search = "Cụm Sân 4",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }

    }
}
