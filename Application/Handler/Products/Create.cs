using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Domain.Enum;
using System.Globalization;

namespace Application.Handler.Products
{
    public class Create
    {
        public class Command : IRequest<Result<ProductDto>>
        {
            public ProductInputDto product { get; set; }
            public string userName { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.product).SetValidator(new ProductValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<ProductDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<ProductDto>> Handle(Command request, CancellationToken cancellationToken)
            {

                if (request.product == null)
                {
                    return Result<ProductDto>.Failure("Product data is required.");
                }

                var product = _mapper.Map<Product>(request.product);

                // Check if the product already exists based on unique criteria (e.g., ProductName or another property)
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(x => x.ProductName.ToLower().Equals(product.ProductName.ToLower())
                    && x.CourtClusterId == product.CourtClusterId
                    && x.CategoryId == product.CategoryId
                     && x.Price == product.Price
                     && x.ImportFee == product.ImportFee, cancellationToken);

                if (existingProduct != null)
                {
                    // If the product exists, increase the quantity by 1
                    var numberQuantity = existingProduct.Quantity;
                    existingProduct.Quantity += request.product.Quantity;

                    // Update the existing product in the database
                    existingProduct.UpdatedAt = DateTime.Now;
                    existingProduct.UpdatedBy = request.userName;
                    _context.Products.Update(existingProduct);
                    var updateResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!updateResult)
                        return Result<ProductDto>.Failure("Failed to update product quantity");

                    // Optionally, log the update
                    var cultureInfo = new CultureInfo("vi-VN")
                    {
                        NumberFormat = { CurrencySymbol = "₫", CurrencyDecimalDigits = 0 }
                    };

                    var productLog = _mapper.Map<ProductLog>(existingProduct);
                    productLog.Id = 0;
                    productLog.CategoryId = existingProduct.CategoryId;
                    productLog.CourtClusterId = existingProduct.CourtClusterId;
                    productLog.ProductId = existingProduct.Id;
                    productLog.CreatedBy = request.userName;
                    productLog.Description = "Đã tăng số lượng của " + existingProduct.ProductName + "từ " + numberQuantity + " thành " + existingProduct.Quantity;
                    productLog.LogType = LogType.Update;

                    await _context.ProductLogs.AddAsync(productLog, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    var updatedProductDto = await _context.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                                                                   .FirstOrDefaultAsync(p => p.Id == existingProduct.Id);

                    return Result<ProductDto>.Success(updatedProductDto);
                }
                else
                {
                    // If the product does not exist, add the new product to the database
                    product.Category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == product.CategoryId);
                    product.CourtCluster = await _context.CourtClusters.FirstOrDefaultAsync(x => x.Id == product.CourtClusterId);

                    await _context.AddAsync(product, cancellationToken);
                    var addResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!addResult)
                        return Result<ProductDto>.Failure("Failed to create product");

                    // Log the product creation
                    var cultureInfo = new CultureInfo("vi-VN")
                    {
                        NumberFormat = { CurrencySymbol = "₫", CurrencyDecimalDigits = 0 }
                    };

                    var productLog = _mapper.Map<ProductLog>(product);
                    productLog.Id = 0;
                    productLog.CategoryId = product.CategoryId;
                    productLog.CourtClusterId = product.CourtClusterId;
                    productLog.ProductId = product.Id;
                    productLog.CreatedBy = request.userName;
                    productLog.Description = "Đã nhập " + product.Quantity + " " + product.ProductName + " với giá nhập là " + string.Format(cultureInfo, "{0:C}", product.ImportFee);
                    productLog.LogType = LogType.Create;

                    await _context.ProductLogs.AddAsync(productLog, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    var newProductDto = await _context.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                                                              .FirstOrDefaultAsync(p => p.Id == product.Id);

                    return Result<ProductDto>.Success(newProductDto);
                }

                // var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == product.CategoryId);
                // var courtCluster = await _context.CourtClusters.FirstOrDefaultAsync(x => x.Id == product.CourtClusterId);
                // var cultureInfo = new CultureInfo("vi-VN")
                // {
                //     NumberFormat = { CurrencySymbol = "₫", CurrencyDecimalDigits = 0 }
                // };
                // product.Category = category;
                // product.CourtCluster = courtCluster;

                // // Thêm product vào database
                // await _context.AddAsync(product, cancellationToken);
                // var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                // if (!result) return Result<ProductDto>.Failure("Failed to create product");

                // // Lưu product vào log sau khi đã lưu thành công
                // var productLog = _mapper.Map<ProductLog>(product); // Dùng AutoMapper để ánh xạ
                // productLog.Id = 0;
                // productLog.CategoryId = product.CategoryId;
                // productLog.CourtClusterId = product.CourtClusterId;
                // productLog.ProductId = product.Id;
                // productLog.CreatedBy = request.userName;
                // productLog.Description = "đã nhập " + product.Quantity + " " + product.ProductName + " với giá nhập là " + string.Format(cultureInfo, "{0:C}", product.ImportFee);
                // productLog.LogType = LogType.Create;

                // // Thêm product log vào database
                // await _context.ProductLogs.AddAsync(productLog, cancellationToken);
                // await _context.SaveChangesAsync(cancellationToken);

                // // Lấy thông tin product để trả về
                // var newProduct = _context.Entry(product).Entity;
                // var data = await _context.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                //                                  .FirstOrDefaultAsync(p => p.Id == newProduct.Id);
                // return Result<ProductDto>.Success(data);
            }
        }
    }
}
