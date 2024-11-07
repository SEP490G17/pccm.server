using Application.DTOs;
using FluentValidation;

namespace Application.Handler.Products
{
    public class ProductValidator : AbstractValidator<ProductInputDto>
    {
        public ProductValidator()
        {
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category is required");
            RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product name is required");
            RuleFor(x => x.PriceBuy).NotEmpty().WithMessage("PriceBuy is required");
            RuleFor(x => x.PriceSell).NotEmpty().WithMessage("PriceSell is required");
            RuleFor(x => x.Quantity).NotEmpty().WithMessage("Quantity is required");
        }
    }
}
