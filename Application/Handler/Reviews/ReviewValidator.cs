using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Reviews
{
    public class ReviewValidator : AbstractValidator<ReviewInputDTO>
    {
        public ReviewValidator() {
            RuleFor(x => x.CourtClusterId).NotEmpty().WithMessage("CourtCluster is required");
            RuleFor(x => x.Comment).NotEmpty().WithMessage("Comment is required");
        }
    }
}
