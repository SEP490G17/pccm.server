

using Domain.Entity;
using FluentValidation;

namespace Application.Handler.Categories
{
    public class CategoryValidator:AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Category Name is required");

        }
    }
}