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

                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == product.CategoryId);
                var courtCluster = await _context.CourtClusters.FirstOrDefaultAsync(x => x.Id == product.CourtClusterId);
                var cultureInfo = new CultureInfo("vi-VN")
                {
                    NumberFormat = { CurrencySymbol = "₫", CurrencyDecimalDigits = 0 }
                };
                product.Category = category;
                product.CourtCluster = courtCluster;

                // Thêm product vào database
                await _context.AddAsync(product, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<ProductDto>.Failure("Failed to create product");

                // Lưu product vào log sau khi đã lưu thành công
                var productLog = _mapper.Map<ProductLog>(product); // Dùng AutoMapper để ánh xạ
                productLog.Id = 0;
                productLog.CategoryId = product.CategoryId;
                productLog.CourtClusterId = product.CourtClusterId;
                productLog.ProductId = product.Id;
                productLog.CreatedBy = request.userName;
                productLog.Description = "đã nhập " + product.Quantity + " " + product.ProductName + " với giá nhập là " + string.Format(cultureInfo, "{0:C}", product.ImportFee);
                productLog.LogType = LogType.Create;

                // Thêm product log vào database
                await _context.ProductLogs.AddAsync(productLog, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // Lấy thông tin product để trả về
                var newProduct = _context.Entry(product).Entity;
                var data = await _context.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                                                 .FirstOrDefaultAsync(p => p.Id == newProduct.Id);
                return Result<ProductDto>.Success(data);
            }
        }
    }
}
