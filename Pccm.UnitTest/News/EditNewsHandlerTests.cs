using API.Extensions;
using Application.Handler.News;
using Domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.News
{
    public class EditNewsHandlerTests
    {
        private readonly IMediator Mediator;

        public EditNewsHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(1, "Thuoc la", "link anh", "ngich ngu", "2024-11-11T10:00:00", "2024-11-11T12:00:00", "Hanoi", ExpectedResult = true)]
        public async Task<bool> Handle_ShouldEditNewsBlog_WhenValidId(
            int id,
            string Title,
            string Thumbnail,
            string Description,
            string StartTime,
            string EndTime,
            string Location)
        {
            try
            {
                // Prepare the NewsBlog input DTO for editing
                var newsBlogInputDto = new NewsBlog()
                {
                    Id = id,
                    Title = Title,
                    Thumbnail = Thumbnail,
                    Description = Description,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    Location = Location,
                    Tags = new string[] { "Tag1", "Tag2" },
                    Status = Domain.Enum.BannerStatus.Display
                };

                var result = await Mediator.Send(new Edit.Command() { Event = newsBlogInputDto }, default);

                // Return if the operation was successful
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(1, null, "link anh", "ngich ngu", "2024-11-11T10:00:00", "2024-11-11T12:00:00", "Hanoi", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditNewsBlogFail_WhenTitleIsNull(
           int id,
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
                    Id = id,
                    Title = Title,
                    Thumbnail = Thumbnail,
                    Description = Description,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    Location = Location,
                    Tags = new string[] { "Tag1", "Tag2" },
                    Status = Domain.Enum.BannerStatus.Display
                };

                var result = await Mediator.Send(new Edit.Command() { Event = newsBlogInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [TestCase(1, "Tin tuc ve san 1", null, "ngich ngu", "2024-11-11T10:00:00", "2024-11-11T12:00:00", "Hanoi", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditNewsBlogFail_WhenThumbnailIsNull(
           int id,
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
                    Id = id,
                    Title = Title,
                    Thumbnail = Thumbnail,
                    Description = Description,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    Location = Location,
                    Tags = new string[] { "Tag1", "Tag2" },
                    Status = Domain.Enum.BannerStatus.Display
                };

                var result = await Mediator.Send(new Edit.Command() { Event = newsBlogInputDto }, default);

                // Return if the operation was successful
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(1, "Tin tuc ve san 1", "link anh", "ngich ngu", null, "2024-11-11T12:00:00", "Hanoi", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditNewsBlogFail_WhenStartTimelIsNull(
           int id,
           string? Title,
           string Thumbnail,
           string Description,
           string? StartTime,
           string EndTime,
           string Location)
        {
            try
            {
                var newsBlogInputDto = new NewsBlog()
                {
                    Id = id,
                    Title = Title,
                    Thumbnail = Thumbnail,
                    Description = Description,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    Location = Location,
                    Tags = new string[] { "Tag1", "Tag2" },
                    Status = Domain.Enum.BannerStatus.Display
                };

                var result = await Mediator.Send(new Edit.Command() { Event = newsBlogInputDto }, default);

                // Return if the operation was successful
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

          [TestCase(1, "Tin tuc ve san 1", "link anh", "ngich ngu", "2024-11-11T10:00:00", null, "Hanoi", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditNewsBlogFail_WhenEndTimelIsNull(
           int id,
           string? Title,
           string Thumbnail,
           string Description,
           string StartTime,
           string? EndTime,
           string Location)
        {
            try
            {
                var newsBlogInputDto = new NewsBlog()
                {
                    Id = id,
                    Title = Title,
                    Thumbnail = Thumbnail,
                    Description = Description,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    Location = Location,
                    Tags = new string[] { "Tag1", "Tag2" },
                    Status = Domain.Enum.BannerStatus.Display
                };

                var result = await Mediator.Send(new Edit.Command() { Event = newsBlogInputDto }, default);

                // Return if the operation was successful
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

          [TestCase(1, "Tin tuc ve san 1", "link anh", "ngich ngu", "2024-11-11T10:00:00", "2024-11-11T10:00:00", null, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditNewsBlogFail_WhenLocationIsNull(
           int id,
           string? Title,
           string Thumbnail,
           string Description,
           string StartTime,
           string EndTime,
           string? Location)
        {
            try
            {
                var newsBlogInputDto = new NewsBlog()
                {
                    Id = id,
                    Title = Title,
                    Thumbnail = Thumbnail,
                    Description = Description,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    Location = Location,
                    Tags = new string[] { "Tag1", "Tag2" },
                    Status = Domain.Enum.BannerStatus.Display
                };

                var result = await Mediator.Send(new Edit.Command() { Event = newsBlogInputDto }, default);

                // Return if the operation was successful
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
