using Application.DTOs;
using FluentValidation;

namespace Application.Handler.CourtClusters
{
    public class CourtClusterValidator:AbstractValidator<CourtClustersInputDto>
    {
        public CourtClusterValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Court cluster name is required");
        }
    }
}