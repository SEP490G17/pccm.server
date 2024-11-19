using API.Extensions;
using Application.Handler.Users;
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


        [TestCase("06b243a8-8158-4a9c-845e-63054506a1b8", ExpectedResult = true)]
        public async Task<bool> Handle_ShouldDetailStaff_WhenStaffExist(
            string id)
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

         [TestCase("06b243a8-8158-4a9c-845e-63054506a1b89", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldDetailStaff_WhenNotExistStaff(
            string id)
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