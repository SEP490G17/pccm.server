using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;
using Application.Handler.Reviews;

namespace Pccm.UnitTest.Reviews
{
    [TestFixture]
    public class CreateReviewHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateReviewHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("0327aee1-dc23-46f1-8a5b-343d669afd85", 4, 5, "Great service!", "2024-11-11T10:00:00", ExpectedResult = true)]
        public async Task<bool> Handle_CreateReview_WhenValid(
            string UserId,
            int CourtClusterId,
            int Rating,
            string Comment,
            string CreatedAt)
        {
            try
            {
                var reviewInputDto = new ReviewInputDto()
                {
                    UserId = UserId,
                    CourtClusterId = CourtClusterId,
                    Rating = Rating,
                    Comment = Comment,
                    CreatedAt = DateTime.Parse(CreatedAt)
                };

                var result = await Mediator.Send(new Create.Command() { review = reviewInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
