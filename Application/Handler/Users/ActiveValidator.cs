using Application.DTOs;
using FluentValidation;


namespace Application.Handler.Users
{
    public class ActiveValidator : AbstractValidator<ActiveDTO>
    {
        public ActiveValidator()
        {

        }
    }
}
