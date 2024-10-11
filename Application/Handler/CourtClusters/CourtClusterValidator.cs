using Application.DTOs;
using Domain.Entity;
using FluentValidation;

namespace Application.Handler.CourtClusters
{
    public class CourtClusterValidator:AbstractValidator<CourtClustersInputDTO>
    {
        public CourtClusterValidator()
        {
            RuleFor(x => x.CourtClusterName).NotEmpty().WithMessage("Court cluster name is required");
            RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required");
        }
    }
}