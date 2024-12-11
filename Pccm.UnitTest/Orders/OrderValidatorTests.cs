using Application.DTOs;
using Application.Handler.Orders;
using FluentValidation.TestHelper;

namespace Pccm.UnitTest.Orders
{
    [TestFixture]
    public class OrderValidatorTests
    {
        private OrderValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new OrderValidator();
        }

        [Test]
        public void Should_Have_Error_When_TotalAmount_Is_Empty()
        {
            // Arrange
            var order = new OrderInputDto
            {
                TotalAmount = 0,
                Status = "Pending",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            // Act
            var result = _validator.TestValidate(order);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TotalAmount);
        }

        [Test]
        public void Should_Have_Error_When_Status_Is_Empty()
        {
            // Arrange
            var order = new OrderInputDto
            {
                TotalAmount = 100,
                Status = "",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            // Act
            var result = _validator.TestValidate(order);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Test]
        public void Should_Have_Error_When_StartTime_Is_Empty()
        {
            // Arrange
            var order = new OrderInputDto
            {
                TotalAmount = 100,
                Status = "Pending",
                StartTime = default,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            // Act
            var result = _validator.TestValidate(order);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.StartTime);
        }

        [Test]
        public void Should_Have_Error_When_EndTime_Is_Empty()
        {
            // Arrange
            var order = new OrderInputDto
            {
                TotalAmount = 100,
                Status = "Pending",
                StartTime = DateTime.UtcNow,
                EndTime = default
            };

            // Act
            var result = _validator.TestValidate(order);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Test]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            // Arrange
            var order = new OrderInputDto
            {
                TotalAmount = 100,
                Status = "Pending",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            // Act
            var result = _validator.TestValidate(order);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TotalAmount);
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
            result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
            result.ShouldNotHaveValidationErrorFor(x => x.EndTime);
        }
    }
}