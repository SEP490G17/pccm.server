using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.Courts;
using Domain.Enum;
using Application.DTOs;

namespace Pccm.UnitTest.Courts
{
    [TestFixture]
    public class CreateCourtHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateCourtHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("Sân 44", 4, CourtStatus.Available, 200000, "08:00", "18:00", ExpectedResult = true)]
        public async Task<bool> Handle_CreateCourt_WhenValid(
               string courtName,
                int courtClusterId,
                CourtStatus status,
                decimal courtPrice,
                string fromTime,
                string toTime)
        {
            try
            {
                var courtPrices = new List<CourtPricesDto>
                {
                    new CourtPricesDto 
                    { 
                        Price = courtPrice, 
                        FromTime = TimeOnly.Parse(fromTime), 
                        ToTime = TimeOnly.Parse(toTime) 
                    }
                };

                var courtInputDto = new CourtCreateDto
                {
                    CourtName = courtName,
                    CourtClusterId = courtClusterId,
                    CourtPrice = courtPrices
                };

                var result = await Mediator.Send(new Create.Command() { Court = courtInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [TestCase("Premium Service 2", 100, CourtStatus.Available, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateCourtFail_WhenNotExistCourtCluster(
                string CourtName,
                int CourtClusterId,
                CourtStatus Status)
        {
            try
            {
                var courtInputDto = new CourtCreateDto
                {
                    CourtName = CourtName,
                    CourtClusterId = CourtClusterId,
                };

                var result = await Mediator.Send(new Create.Command() { Court = courtInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
