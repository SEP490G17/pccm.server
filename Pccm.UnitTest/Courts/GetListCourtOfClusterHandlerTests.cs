using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;

namespace Pccm.UnitTest.Courts
{
    public class GetListCourtOfClusterHandlerTests
    {
        public readonly IMediator Mediator;

        public GetListCourtOfClusterHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase(0, 5, 5, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListCourt_WhenValid(int skip, int pageSize, int courtClusterId)
        {
            if (this.Mediator == null) return null;

            // Gửi yêu cầu truy vấn danh sách sân của cụm sân
            var response = await this.Mediator.Send(new Application.Handler.Courts.GetListCourtOfCluster.Query()
            {
                CourtClusterId = courtClusterId
            });

            return response.IsSuccess;
        }

        [TestCase(0, 5, 111, ExpectedResult = false)]
        public async Task<bool?> Handle_ShouldListCout_WhenNotExistCourtID(int skip, int pageSize, int courtClusterId)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Courts.GetListCourtOfCluster.Query()
            {
                CourtClusterId = courtClusterId
            });

            // Kiểm tra nếu kết quả thành công
            return response.IsSuccess;
            
        }

    }
}
