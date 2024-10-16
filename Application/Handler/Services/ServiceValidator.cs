using Application.DTOs;
using Domain.Entity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Services
{
    public class ServiceValidator : AbstractValidator<ServiceDto>
    {
        public ServiceValidator()
        {
            RuleFor(x => x.ServiceName).NotEmpty().WithMessage("Service name is required");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Price is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
        }
    }
}
