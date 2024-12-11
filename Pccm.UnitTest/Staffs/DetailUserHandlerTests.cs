using API.Extensions;
using Application.Handler.Staffs;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Staffs
{
    public class DetailStaffHandlerTests
    {
            private readonly IMediator Mediator;

        public DetailStaffHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(5, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailStaff_WhenStaffExist(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Detail.Query() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase(544, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDetailStaff_WhenNotExistStaff(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Detail.Query() { Id = id }, default);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}