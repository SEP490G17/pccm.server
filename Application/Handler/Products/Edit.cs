using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Products
{
    public class Edit
    {
        public class Command : IRequest<Result<ProductDto>>
        {
            public ProductInputDto product { get; set; }
            public int Id { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.product).SetValidator(new ProductValidator());
            }
        }
        public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<ProductDto>>
        {

            public async Task<Result<ProductDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var product = mapper.Map<Product>(request.product);
                var repo = unitOfWork.Repository<Product>();
                var productToUpdate = await repo.GetByIdAsync(request.Id);
                if (productToUpdate == null)
                {
                    return Result<ProductDto>.Failure("Product not found");
                }
                mapper.Map(product, productToUpdate);
                productToUpdate.UpdatedAt = DateTime.Now;
                productToUpdate.UpdatedBy = "anonymous";
                repo.Update(productToUpdate);
                var result = await unitOfWork.Complete() > 0;
                if (!result)
                {
                    return Result<ProductDto>.Failure("Failed to edit product");
                }
                var response = await repo.QueryList(null)
                    .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(p => p.Id == productToUpdate.Id);

                return Result<ProductDto>.Success(response);
            }
        }
    }
}
