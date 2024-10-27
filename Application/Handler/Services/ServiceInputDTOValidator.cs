using Application.DTOs;
using FluentValidation;

namespace Application.Handler.Services
{
    public class ServiceInputDTOValidator : AbstractValidator<ServiceInputDTO>
    {
        public ServiceInputDTOValidator()
        {
            RuleFor(x => x.ServiceName).NotEmpty().WithMessage("Service name is required");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Price is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
        }
    }
}
