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
    public class ImportProduct
    {
        public class Command : IRequest<Result<ProductDto>>
        {
            public ProductImportDto product { get; set; }
            public int Id { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.product).SetValidator(new ProductImportValidator());
            }
        }
        public class Handler(IUnitOfWork unitOfWork, IMapper mapper, IUserAccessor userAccessor, IMediator mediator) : IRequestHandler<Command, Result<ProductDto>>
        {
            public async Task<Result<ProductDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var productImport = request.product;
                var userName = userAccessor.GetUserName();
                var repo = unitOfWork.Repository<Product>();
                var cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                cultureInfo.NumberFormat.CurrencySymbol = "₫";
                cultureInfo.NumberFormat.CurrencyDecimalDigits = 0;
                cultureInfo.NumberFormat.NumberGroupSeparator = ".";
                cultureInfo.NumberFormat.CurrencyGroupSeparator = ".";
                var productToUpdate = await repo.GetByIdAsync(request.Id);

                if (productToUpdate == null)
                {
                    return Result<ProductDto>.Failure("Không tìm thấy sản phẩm");
                }

                List<int> courtClusterId = await mediator.Send(new GetCurrentStaffCluster.Query(), cancellationToken);
                if (courtClusterId != null && !courtClusterId.Contains((int)productToUpdate.CourtClusterId))
                {
                    return Result<ProductDto>.Failure("Bạn không có quyền thực hiện quyền này");
                }
                string description = $"đã nhập thêm {productImport.Quantity} {productToUpdate.ProductName}. ";

                decimal newImportFee = (productImport.ImportFee * productImport.Quantity + productToUpdate.ImportFee * productToUpdate.Quantity) / (productImport.Quantity + productToUpdate.Quantity);

                description += $"Giá nhập thay đổi {string.Format(cultureInfo, "{0:C}", productToUpdate.ImportFee)} thành {string.Format(cultureInfo, "{0:C}", newImportFee)} ";

                productToUpdate.Quantity += productImport.Quantity;
                productToUpdate.ImportFee = newImportFee;
                //log
                var productLog = mapper.Map<ProductLog>(productToUpdate);
                productLog.Id = 0;
                productLog.CreatedBy = userName;
                productLog.CreatedAt = DateTime.Now;
                productLog.Description = description;
                productLog.Price = productImport.ImportFee;
                productLog.LogType = LogType.Create;

                var logRepo = unitOfWork.Repository<ProductLog>();
                logRepo.Add(productLog);

                productToUpdate.UpdatedAt = DateTime.Now;
                productToUpdate.UpdatedBy = userName;

                repo.Update(productToUpdate);
                var result = await unitOfWork.Complete() > 0;

                if (!result)
                {
                    return Result<ProductDto>.Failure("Failed to import product");
                }

                var response = await repo.QueryList(null)
                    .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(p => p.Id == productToUpdate.Id);

                return Result<ProductDto>.Success(response);
            }
        }
    }
}
