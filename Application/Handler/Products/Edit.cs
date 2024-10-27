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
        public class Command : IRequest<Result<ProductDTO>>
        {
            public ProductInputDTO product { get; set; }
            public int Id { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.product).SetValidator(new ProductValidator());
            }
        }
        public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<ProductDTO>>
        {

            public async Task<Result<ProductDTO>> Handle(Command request, CancellationToken cancellationToken)
            {
                var productUpdate = request.product;
                var id = request.Id;
                var repo = unitOfWork.Repository<Product>();
                var product = await repo.GetByIdAsync(id);
                mapper.Map(productUpdate,product);
                repo.Update(product);
                var result = await unitOfWork.Complete() > 0;
                if (!result) return Result<ProductDTO>.Failure("Faild to edit product");
                var response = await repo.QueryList(null).ProjectTo<ProductDTO>(mapper.ConfigurationProvider).FirstOrDefaultAsync(p=>p.Id == product.Id);
                return Result<ProductDTO>.Success(response);
            }
        }
    }
}
