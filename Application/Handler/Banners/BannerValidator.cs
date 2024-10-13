using Domain.Entity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Banners
{
    public class BannerValidator:AbstractValidator<Banner>
    {
        public BannerValidator() {
            RuleFor(x => x.LinkUrl).NotEmpty().WithMessage("Link url is required");
            RuleFor(x => x.ImageUrl).NotEmpty().WithMessage("Image is required");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Start date is required");
            RuleFor(x => x.EndDate).NotEmpty().WithMessage("End date is required");
        }
    }
}
