using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            RuleFor(x => x.CourtId).NotEmpty().WithMessage("Court Id is required");
            RuleFor(x => x.CourtClusterId).NotEmpty().WithMessage("CourtCluster Id is required");
            RuleFor(x => x.RecurrenceRule).NotEmpty().WithMessage("Recurrence rule is required");
        }
    }
}

