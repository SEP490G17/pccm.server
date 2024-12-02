using API.Extensions;
using Application.DTOs;
using Application.Handler.Reviews;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Reviews
{
    public class EditServiceHandlerTests
    {
        private readonly IMediator Mediator;

        public EditServiceHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(5, 4, 4, "Great service!", "2024-11-11T10:00:00", ExpectedResult = true)]
        public async Task<bool> Handle_EditReview_WhenValid(
            int id, 
            int CourtClusterId,
            int Rating,
            string Comment,
            string CreatedAt)
        {
            try
            {
                var reviewInputDto = new ReviewInputDto()
                {
                    Id = id,
                    CourtClusterId = CourtClusterId,
                    Rating = Rating,
                    Comment = Comment,
                    CreatedAt = DateTime.Parse(CreatedAt)
                };

                var result = await Mediator.Send(new Edit.Command() { review = reviewInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
