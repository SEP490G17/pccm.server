using Domain.Entity;
using FluentValidation;

namespace Application.Handler.Courts
{
    public class CourtValidator:AbstractValidator<Court>
    {
        public CourtValidator()
        {
            RuleFor(x => x.CourtName).NotEmpty().WithMessage("CourtName is required");
        }
    }
}