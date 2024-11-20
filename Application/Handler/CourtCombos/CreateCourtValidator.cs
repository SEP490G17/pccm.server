

using Application.DTOs;
using FluentValidation;

namespace Application.Handler.CourtCombos
{
    public class CreateCourtValidator : AbstractValidator<CreateCourtCombo.Command>
    {
        public CreateCourtValidator()
        {
            RuleFor(x => x.CourtComboCreateDtos)
                 .NotNull().WithMessage("Court combos list cannot be null.")
                 .NotEmpty().WithMessage("Court combos list cannot be empty.")
                 .ForEach(x => x.SetValidator(new CourtComboCreateDtoValidator()));
        }
    }
    public class CourtComboCreateDtoValidator : AbstractValidator<CourtComboCreateDto>
    {
        public CourtComboCreateDtoValidator()
        {
            // Xác thực các trường trong CourtComboCreateDto
            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .MaximumLength(100).WithMessage("Display name can't be longer than 100 characters.");

            RuleFor(x => x.TotalPrice)
                .GreaterThan(0).WithMessage("Total price must be greater than zero.");

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than zero.");


            RuleFor(x => x.CourtId)
                .GreaterThan(0).WithMessage("CourtId must be greater than zero.");
        }
    }
}