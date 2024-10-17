using Domain.Entity;
using FluentValidation;

namespace Application.Handler.News
{
    public class EventValidator:AbstractValidator<NewsBlog>
    {
        public EventValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.StartTime).NotEmpty().WithMessage("StartTime is required");
            RuleFor(x => x.EndTime).NotEmpty().WithMessage("EndTime is required");
            RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required");
        }
    }
}