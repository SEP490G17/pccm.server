using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;
using Application.DTOs;

namespace Pccm.UnitTest.Services
{
    public class UpdateRoleHandlerTests
    {
        public readonly IMediator Mediator;

        public UpdateRoleHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(0, 5, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListService_WhenValid(int skip, int pageSize)
        {
            if (this.Mediator is null) return null;

            // Tạo dữ liệu đầu vào
            var inputData = new List<StaffRoleInputDto>
            {
                new StaffRoleInputDto
                {
                    name = "Quản lý",
                    defaultRoles = new[] { "ManagerOrder", "OWNER", "MANAGERCUSTOMER" }
                },
                new StaffRoleInputDto
                {
                    name = "Staff",
                    defaultRoles = new[] { "ADMIN", "OWNER" }
                }
            };

           var response = await this.Mediator.Send(new Application.Handler.StaffPositions.UpdateRole.Command
            {
                data = inputData
            });

            return response.IsSuccess;
        }

    }
}
