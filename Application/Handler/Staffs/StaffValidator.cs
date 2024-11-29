using Application.DTOs;
using Domain.Entity;
using FluentValidation;

namespace Application.Handler.Staffs
{
    public class StaffValidator : AbstractValidator<StaffInputDto>
    {
        public StaffValidator()
        {
            RuleFor(x => x.userName).NotEmpty().WithMessage("userName is required");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is required");
            RuleFor(x => x.PositionId).NotEmpty().WithMessage("PositionId is required");
        }
    }
}
