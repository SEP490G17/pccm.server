using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                StartTime = DateTime.MinValue, 
                EndTime = DateTime.Now.AddHours(1), 
                PhoneNumber = "0987654321", 
                FullName = "John Doe", 
                CourtId = 1 
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.StartTime);
        }

        [Test]
        public void Should_Have_Error_When_EndTime_Is_Empty()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, 
                EndTime = DateTime.MinValue, 
                PhoneNumber = "0987654321", 
                FullName = "John Doe", 
                CourtId = 1 
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Test]
        public void Should_Have_Error_When_PhoneNumber_Is_Empty()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, 
                EndTime = DateTime.Now.AddHours(1),
                PhoneNumber = string.Empty, 
                FullName = "John Doe", 
                CourtId = 1 
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Test]
        public void Should_Have_Error_When_FullName_Is_Empty()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, 
                EndTime = DateTime.Now.AddHours(1),
                PhoneNumber = "0987654321", 
                FullName = string.Empty, 
                CourtId = 1 
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Test]
        public void Should_Have_Error_When_CourtId_Is_Empty()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, 
                EndTime = DateTime.Now.AddHours(1), 
                PhoneNumber = "0987654321", 
                FullName = "John Doe", 
                CourtId = 0 
            };

            var result = _validator.TestValidate(booking);
            result.ShouldHaveValidationErrorFor(x => x.CourtId);
        }

        [Test]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var booking = new BookingInputDto
            {
                StartTime = DateTime.Now, 
                EndTime = DateTime.Now.AddHours(1), 
                PhoneNumber = "0987654321", 
                FullName = "John Doe", 
                CourtId = 1 
            };

            var result = _validator.TestValidate(booking);
            result.ShouldNotHaveAnyValidationErrors();
        } 
    }
}