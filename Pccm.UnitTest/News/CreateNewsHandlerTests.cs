using Application.Handler.News;
using Domain.Entity;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Pccm.UnitTest.News
{
    [TestFixture]
    public class CreateNewsHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateNewsHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("Thuoc la", "link anh", "ngich ngu", "2024-11-11T10:00:00", "2024-11-11T12:00:00", "Hanoi", ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCreateNewsBlog_WhenValidId(
            string Title,
            string Thumbnail,
            string Description,
            string StartTime,
            string EndTime,
            string Location)
        {
            try
            {
                var newsBlogInputDto = new NewsBlog()
                {
                    Title = Title,
                    Thumbnail = Thumbnail,
                    Description = Description,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    Location = Location,
                    Tags = new string[] { "Tag1", "Tag2" },
                    Status = Domain.Enum.BannerStatus.Display
                };

                var result = await Mediator.Send(new Create.Command() { Event = newsBlogInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(null, "link anh", "ngich ngu", "2024-11-11T10:00:00", "2024-11-11T12:00:00", "Hanoi", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateNewsBlogFail_WhenTitleIsNull(
           string? Title,
           string Thumbnail,
           string Description,
           string StartTime,
           string EndTime,
           string Location)
        {
            try
            {
                var newsBlogInputDto = new NewsBlog()
                {
                    Title = Title,
                    Thumbnail = Thumbnail,
                    Description = Description,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    Location = Location,
                    Tags = new string[] { "Tag1", "Tag2" },
                    Status = Domain.Enum.BannerStatus.Display
                };

                var result = await Mediator.Send(new Create.Command() { Event = newsBlogInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("Dich vu vip", null, "ngich ngu", "2024-11-11T10:00:00", "2024-11-11T12:00:00", "Hanoi", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateNewsBlogFail_WhenThumbnailIsNull(
          string? Title,
          string? Thumbnail,
          string Description,
          string StartTime,
          string EndTime,
          string Location)
        {
            try
            {
                var newsBlogInputDto = new NewsBlog()
                {
                    Title = Title,
                    Thumbnail = Thumbnail,
                    Description = Description,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    Location = Location,
                    Tags = new string[] { "Tag1", "Tag2" },
                    Status = Domain.Enum.BannerStatus.Display
                };

                var result = await Mediator.Send(new Create.Command() { Event = newsBlogInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
