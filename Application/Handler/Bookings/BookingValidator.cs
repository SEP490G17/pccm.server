using Application.DTOs;
using FluentValidation;

namespace Application.Handler.Bookings
{
    public class BookingValidator : AbstractValidator<BookingInputDto>
    {
        public BookingValidator()
        {
            RuleFor(x => x.StartTime).NotEmpty().WithMessage("Start time is required");
            RuleFor(x => x.EndTime).NotEmpty().WithMessage("End time is required");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("FullName is required");
            RuleFor(x => x.CourtId).NotEmpty().WithMessage("Court Id is required");
        }
    }
}

