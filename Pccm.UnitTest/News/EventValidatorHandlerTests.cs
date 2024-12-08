using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using Application.SpecParams;
using Domain.Entity;
using Application.Handler.News;
using FluentValidation.TestHelper;

namespace Pccm.UnitTest.News
{
    [TestFixture]
    public class EventValidatorHandlerTests
    {
         private EventValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new EventValidator();
        }

        [Test]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var newsBlog = new NewsBlog
            {
                Title = "",
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(2),
                Location = "Some Location"
            };

            var result = _validator.TestValidate(newsBlog);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required");
        }

        [Test]
        public void Should_Have_Error_When_StartTime_Is_Empty()
        {
            var newsBlog = new NewsBlog
            {
                Title = "Some Title",
                StartTime = DateTime.MinValue,  // Invalid StartTime
                EndTime = DateTime.Now.AddDays(2),
                Location = "Some Location"
            };

            var result = _validator.TestValidate(newsBlog);

            result.ShouldHaveValidationErrorFor(x => x.StartTime)
                  .WithErrorMessage("StartTime is required");
        }

        [Test]
        public void Should_Have_Error_When_EndTime_Is_Empty()
        {
            var newsBlog = new NewsBlog
            {
                Title = "Some Title",
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.MinValue,  // Invalid EndTime
                Location = "Some Location"
            };

            var result = _validator.TestValidate(newsBlog);

            result.ShouldHaveValidationErrorFor(x => x.EndTime)
                  .WithErrorMessage("EndTime is required");
        }

        [Test]
        public void Should_Have_Error_When_Location_Is_Empty()
        {
            var newsBlog = new NewsBlog
            {
                Title = "Some Title",
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(2),
                Location = ""  // Invalid Location
            };

            var result = _validator.TestValidate(newsBlog);

            result.ShouldHaveValidationErrorFor(x => x.Location)
                  .WithErrorMessage("Location is required");
        }

        [Test]
        public void Should_Pass_Validation_When_All_Fields_Are_Valid()
        {
            var newsBlog = new NewsBlog
            {
                Title = "Some Title",
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(2),
                Location = "Some Location"
            };

            var result = _validator.TestValidate(newsBlog);

            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
            result.ShouldNotHaveValidationErrorFor(x => x.EndTime);
            result.ShouldNotHaveValidationErrorFor(x => x.Location);
        }
    }
}
