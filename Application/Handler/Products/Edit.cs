using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Reflection;

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
        public class Handler(IUnitOfWork unitOfWork, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Command, Result<ProductDto>>
        {
            public async Task<Result<ProductDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                string userName = userAccessor.GetUserName();

                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                var repo = unitOfWork.Repository<Product>();
                var productToUpdate = await repo.GetByIdAsync(request.Id);
                if (productToUpdate == null)
                {
                    return Result<ProductDto>.Failure("Product not found");
                }

                var productLog = mapper.Map<ProductLog>(productToUpdate);
                productLog.Id = 0; 
                productLog.ProductId = productToUpdate.Id;
                productLog.CreatedBy = userName;
                productLog.CreatedAt = vietnamTime;
                productLog.Description = "The price of the product has been changed from " + productToUpdate.Price + " to " + request.product.PriceSell;
                productLog.LogType = LogType.Update;

                var logRepo = unitOfWork.Repository<ProductLog>();
                logRepo.Add(productLog);

                mapper.Map(request.product, productToUpdate);
                productToUpdate.UpdatedAt = vietnamTime;
                productToUpdate.UpdatedBy = userName;

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
