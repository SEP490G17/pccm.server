using Application.DTOs;
using Application.Handler.CourtCombos;
using FluentValidation.TestHelper;

namespace Pccm.UnitTest.Notifications
{
    [TestFixture]
    public class CreateCourtValidatorHandlerTests
    {
      private CreateCourtValidator _commandValidator;
        private CourtComboCreateDtoValidator _comboValidator;

        [SetUp]
        public void Setup()
        {
            _commandValidator = new CreateCourtValidator();
            _comboValidator = new CourtComboCreateDtoValidator();
        }

        [Test]
        public void Should_HaveError_When_CourtComboCreateDtosIsNull()
        {
            var command = new CreateCourtCombo.Command
            {
                CourtId = 1,
                CourtComboCreateDtos = null
            };

            var result = _commandValidator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CourtComboCreateDtos)
                .WithErrorMessage("Court combos list cannot be null.");
        }

        [Test]
        public void Should_HaveError_When_CourtComboCreateDtosIsEmpty()
        {
            var command = new CreateCourtCombo.Command
            {
                CourtId = 1,
                CourtComboCreateDtos = new List<CourtComboDto>()
            };

            var result = _commandValidator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CourtComboCreateDtos)
                .WithErrorMessage("Court combos list cannot be empty.");
        }

        [Test]
        public void Should_NotHaveError_When_ValidCourtComboListIsProvided()
        {
            var command = new CreateCourtCombo.Command
            {
                CourtId = 1,
                CourtComboCreateDtos = new List<CourtComboDto>
                {
                    new CourtComboDto
                    {
                        DisplayName = "Valid Combo",
                        TotalPrice = 100.50m,
                        Duration = 60
                    }
                }
            };

            var result = _commandValidator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.CourtComboCreateDtos);
        }

        [Test]
        public void Should_HaveError_When_DisplayNameIsEmpty()
        {
            var comboDto = new CourtComboDto
            {
                DisplayName = "",
                TotalPrice = 100.50m,
                Duration = 60
            };

            var result = _comboValidator.TestValidate(comboDto);
            result.ShouldHaveValidationErrorFor(c => c.DisplayName)
                .WithErrorMessage("Display name is required.");
        }

        [Test]
        public void Should_HaveError_When_DisplayNameExceedsMaxLength()
        {
            var comboDto = new CourtComboDto
            {
                DisplayName = new string('a', 101),
                TotalPrice = 100.50m,
                Duration = 60
            };

            var result = _comboValidator.TestValidate(comboDto);
            result.ShouldHaveValidationErrorFor(c => c.DisplayName)
                .WithErrorMessage("Display name can't be longer than 100 characters.");
        }

        [Test]
        public void Should_HaveError_When_TotalPriceIsNotGreaterThanZero()
        {
            var comboDto = new CourtComboDto
            {
                DisplayName = "Valid Combo",
                TotalPrice = 0,
                Duration = 60
            };

            var result = _comboValidator.TestValidate(comboDto);
            result.ShouldHaveValidationErrorFor(c => c.TotalPrice)
                .WithErrorMessage("Total price must be greater than zero.");
        }

        [Test]
        public void Should_HaveError_When_DurationIsNotGreaterThanZero()
        {
            var comboDto = new CourtComboDto
            {
                DisplayName = "Valid Combo",
                TotalPrice = 100.50m,
                Duration = 0
            };

            var result = _comboValidator.TestValidate(comboDto);
            result.ShouldHaveValidationErrorFor(c => c.Duration)
                .WithErrorMessage("Duration must be greater than zero.");
        }

        [Test]
        public void Should_NotHaveError_When_AllFieldsAreValid()
        {
            var comboDto = new CourtComboDto
            {
                DisplayName = "Valid Combo",
                TotalPrice = 100.50m,
                Duration = 60
            };

            var result = _comboValidator.TestValidate(comboDto);
            result.ShouldNotHaveValidationErrorFor(c => c.DisplayName);
            result.ShouldNotHaveValidationErrorFor(c => c.TotalPrice);
            result.ShouldNotHaveValidationErrorFor(c => c.Duration);
        }
    }

}

