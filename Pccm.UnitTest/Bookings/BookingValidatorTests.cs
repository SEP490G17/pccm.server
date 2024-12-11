using Application.DTOs;
using Application.Handler.Bookings;
using FluentValidation.TestHelper;

namespace Pccm.UnitTest.Bookings
{
    public class BookingValidatorTests
    {
       
        private BookingValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new BookingValidator();
        }

        [Test]
        public void Should_Have_Error_When_StartTime_Is_Empty()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.MinValue, // Invalid StartTime
                EndTime = DateTime.Now.AddHours(1), // Valid EndTime
                PhoneNumber = "0987654321", // Valid PhoneNumber
                FullName = "John Doe", // Valid FullName
                CourtId = 1 // Valid CourtId
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.StartTime);
        }

        [Test]
        public void Should_Have_Error_When_EndTime_Is_Empty()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, // Valid StartTime
                EndTime = DateTime.MinValue, // Invalid EndTime
                PhoneNumber = "0987654321", // Valid PhoneNumber
                FullName = "John Doe", // Valid FullName
                CourtId = 1 // Valid CourtId
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Test]
        public void Should_Have_Error_When_PhoneNumber_Is_Empty()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, // Valid StartTime
                EndTime = DateTime.Now.AddHours(1), // Valid EndTime
                PhoneNumber = string.Empty, // Invalid PhoneNumber
                FullName = "John Doe", // Valid FullName
                CourtId = 1 // Valid CourtId
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Test]
        public void Should_Have_Error_When_FullName_Is_Empty()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, // Valid StartTime
                EndTime = DateTime.Now.AddHours(1), // Valid EndTime
                PhoneNumber = "0987654321", // Valid PhoneNumber
                FullName = string.Empty, // Invalid FullName
                CourtId = 1 // Valid CourtId
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Test]
        public void Should_Have_Error_When_CourtId_Is_Empty()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, // Valid StartTime
                EndTime = DateTime.Now.AddHours(1), // Valid EndTime
                PhoneNumber = "0987654321", // Valid PhoneNumber
                FullName = "John Doe", // Valid FullName
                CourtId = 0 // Invalid CourtId
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.CourtId);
        }

        [Test]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, // Valid StartTime
                EndTime = DateTime.Now.AddHours(1), // Valid EndTime
                PhoneNumber = "0987654321", // Valid PhoneNumber
                FullName = "John Doe", // Valid FullName
                CourtId = 1 // Valid CourtId
            };

            var result = _validator.TestValidate(booking);
            result.ShouldNotHaveAnyValidationErrors();
        } 
    }
}