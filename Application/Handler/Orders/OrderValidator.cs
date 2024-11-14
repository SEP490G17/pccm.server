using Application.DTOs;
using FluentValidation;

namespace Application.Handler.Orders
{
    public class OrderValidator:AbstractValidator<OrderInputDto>
    {
        public OrderValidator()
        {
            RuleFor(x => x.TotalAmount).NotEmpty().WithMessage("Total amount is required");
            RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required");
            RuleFor(x => x.StartTime).NotEmpty().WithMessage("StartTime is required");
            RuleFor(x => x.EndTime).NotEmpty().WithMessage("EndTime is required");
        }
    }

     public class OrderValidatorV1:AbstractValidator<OrderCreateV1.Command>
    {
        public OrderValidatorV1()
        {
            RuleFor(x => x.BookingId).NotEmpty().WithMessage("Booking lên không được rỗng");
            RuleFor(x => x.BookingId).NotNull().WithMessage("Booking lên không được null");
        }
    }
}