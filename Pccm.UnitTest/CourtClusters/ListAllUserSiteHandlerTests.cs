using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams.CourtClusterSpecification;

namespace Pccm.UnitTest.CourtClusters
{
    public class ListAllUserSiteHandlerTests
    {
        public readonly IMediator Mediator;

        public ListAllUserSiteHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase(0, 5, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListCoutCluster_WhenValid(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.CourtClusters.ListAllUserSite.Query()
            {
                CourtClusterSpecParam = new CourtClusterSpecParam()
                {
                    Skip = 0,
                    PageSize = 12
                }
            });

            return response.IsSuccess;
        }


        //  [TestCase(0, 5, ExpectedResult = 1)]
        // public async Task<int?> Handle_ShouldListCoutCluster_WhenFilterByProvince(int skip, int pageSize)
        // {
        //     if (this.Mediator is null) return null;
        //     var response = await this.Mediator.Send(new Application.Handler.CourtClusters.ListAllUserSite.Query()
        //     {
        //         CourtClusterSpecParam = new CourtClusterSpecParam()
        //         {
        //             Search = "",
        //             Skip = 0,
        //             PageSize = 5
        //         }
        //     });

        //     return response.Value.Data.Count();
        // }

    }
}
