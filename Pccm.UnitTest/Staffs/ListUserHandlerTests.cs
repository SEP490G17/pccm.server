using Application.SpecParams.ProductSpecification;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;

namespace Pccm.UnitTest.Staffs
{
    public class ListStaffHandlerTests
    {
        public readonly IMediator Mediator;

        public ListStaffHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }



        [TestCase(0, 5, ExpectedResult = 5)]
        public async Task<int?> Handle_ShouldListStaff(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Staffs.List.Query()
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

        [TestCase(0, 5, ExpectedResult = 2)]
        public async Task<int?> Handle_ShouldListStaff_WhenSearch(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Staffs.List.Query()
            {
                BaseSpecWithFilterParam = new BaseSpecWithFilterParam()
                {
                    Search = "staff1",
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }

        [TestCase(0, 5, ExpectedResult = 1)]
        public async Task<int?> Handle_ShouldListStaff_WhenFilterByShiftID(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Staffs.List.Query()
            {
                BaseSpecWithFilterParam = new BaseSpecWithFilterParam()
                {
                    Search = "",
                    Filter = 1,
                    Skip = 0,
                    PageSize = 5
                }
            });

            return response.Value.Data.Count();
        }

    }
}
