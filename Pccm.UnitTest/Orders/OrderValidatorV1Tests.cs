using Application.Handler.Orders;
using FluentValidation.TestHelper;

namespace Pccm.UnitTest.Orders
{
    [TestFixture]
    public class OrderValidatorV1Tests
    {
        private OrderValidatorV1 _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new OrderValidatorV1();
        }

        [Test]
        public void Should_Have_Error_When_BookingId_Is_Empty()
        {
            // Arrange
            var command = new OrderCreateV1.Command
            {
                BookingId = 0 // Empty BookingId
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BookingId);
        }

        // [Test]
        // public void Should_Have_Error_When_BookingId_Is_Null()
        // {
        //     // Arrange
        //     var command = new OrderCreateV1.Command
        //     {
        //         BookingId = null // Null BookingId
        //     };

        //     // Act
        //     var result = _validator.TestValidate(command);

        //     // Assert
        //     result.ShouldHaveValidationErrorFor(x => x.BookingId);
        // }

        [Test]
        public void Should_Not_Have_Error_When_BookingId_Is_Valid()
        {
            // Arrange
            var command = new OrderCreateV1.Command
            {
                BookingId = 1 // Valid BookingId
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.BookingId);
        }
    }
}