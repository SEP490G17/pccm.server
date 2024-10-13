using Application.DTOs;
using Domain.Entity;
using FluentValidation;

namespace Application.Handler.Orders
{
    public class OrderValidator:AbstractValidator<OrderInputDTO>
    {
        public OrderValidator()
        {
            RuleFor(x => x.TotalAmount).NotEmpty().WithMessage("Total amount is required");
            RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required");
            RuleFor(x => x.StartTime).NotEmpty().WithMessage("StartTime is required");
            RuleFor(x => x.EndTime).NotEmpty().WithMessage("EndTime is required");
        }
    }
}