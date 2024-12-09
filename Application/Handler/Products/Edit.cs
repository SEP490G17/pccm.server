using System.Globalization;
using Application.Core;
using Application.DTOs;
using Application.Handler.StaffPositions;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        public class Handler(IUnitOfWork unitOfWork, IMapper mapper, IUserAccessor userAccessor, IMediator mediator) : IRequestHandler<Command, Result<ProductDto>>
        {
            public async Task<Result<ProductDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var product = request.product;
                var userName = userAccessor.GetUserName();
                
                List<int> courtClusterId = await mediator.Send(new GetCurrentStaffCluster.Query(), cancellationToken);
                if (courtClusterId != null && !courtClusterId.Contains(request.product.CourtClusterId))
                {
                    return Result<ProductDto>.Failure("Bạn không có quyền thực hiện quyền này");
                }
                var repo = unitOfWork.Repository<Product>();
                var cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                cultureInfo.NumberFormat.CurrencySymbol = "₫";
                cultureInfo.NumberFormat.CurrencyDecimalDigits = 0;
                cultureInfo.NumberFormat.NumberGroupSeparator = ".";
                cultureInfo.NumberFormat.CurrencyGroupSeparator = ".";
                var productToUpdate = await repo.GetByIdAsync(request.Id);
                if (productToUpdate == null)
                {
                    return Result<ProductDto>.Failure("Product not found");
                }
                string description = "đã thay đổi ";
                if (productToUpdate.ProductName != product.ProductName)
                {
                    description += $"tên sản phẩm {productToUpdate.ProductName} thành {product.ProductName} ";
                }
                else if (productToUpdate.CategoryId != product.CategoryId)
                {
                    var repoCate = unitOfWork.Repository<Category>();
                    var category = await repoCate.GetByIdAsync(product.CategoryId);
                    description += $"thể loại sản phẩm {productToUpdate.Category.CategoryName} thành {category.CategoryName} ";
                }
                else if (productToUpdate.CourtClusterId != product.CourtClusterId)
                {
                    var repoCourtCluster = unitOfWork.Repository<CourtCluster>();
                    var courtCluster = await repoCourtCluster.GetByIdAsync(product.CourtClusterId);
                    description += $"cụm sân {productToUpdate.CourtCluster.CourtClusterName} thành {courtCluster.CourtClusterName} ";
                }
                else if (productToUpdate.Price != product.Price)
                {
                    description += $"giá bán {string.Format(cultureInfo, "{0:C}", productToUpdate.Price)} thành {string.Format(cultureInfo, "{0:C}", product.Price)} ";
                }

                //log
                var productLog = mapper.Map<ProductLog>(productToUpdate);
                productLog.Id = 0;
                productLog.CreatedBy = userName;
                productLog.CreatedAt = DateTime.Now;
                productLog.Description = description;
                productLog.Price = productToUpdate.Price;
                productLog.LogType = LogType.Update;

                var logRepo = unitOfWork.Repository<ProductLog>();
                logRepo.Add(productLog);

                mapper.Map(request.product, productToUpdate);
                productToUpdate.UpdatedAt = DateTime.Now;
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
