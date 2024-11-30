using Application.DTOs;
using FluentValidation;

namespace Application.Handler.Products
{
    public class ProductImportValidator : AbstractValidator<ProductImportDto>
    {
        public ProductImportValidator()
        {
            RuleFor(x => x.ImportFee).NotEmpty().WithMessage("ImportFee is required");
            RuleFor(x => x.Quantity).NotEmpty().WithMessage("Quantity is required");
        }
    }
}
