using Application.DTOs;
using FluentValidation;

namespace Application.Handler.Reviews
{
    public class ReviewValidator : AbstractValidator<ReviewInputDto>
    {
        public ReviewValidator() {
            RuleFor(x => x.CourtClusterId).NotEmpty().WithMessage("CourtCluster is required");
            RuleFor(x => x.Comment).NotEmpty().WithMessage("Comment is required");
        }
    }
}
