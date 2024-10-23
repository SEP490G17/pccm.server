using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Products
{
    public class Create
    {
        public class Command : IRequest<Result<ProductDTO>>
        {
            public ProductInputDTO product { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.product).SetValidator(new ProductValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<ProductDTO>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<ProductDTO>> Handle(Command request, CancellationToken cancellationToken)
            {
                var product = _mapper.Map<Product>(request.product);
                var category = await _context.Categories.FirstOrDefaultAsync(x=>x.Id == product.CategoryId);
                var courtCluster = await _context.CourtClusters.FirstOrDefaultAsync(x=>x.Id == product.CourtClusterId);
                product.Category = category;
                product.CourtCluster = courtCluster;
                await _context.AddAsync(product, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<ProductDTO>.Failure("Fail to create product");
                var newProduct = _context.Entry(product).Entity;
                var data = await _context.Products.ProjectTo<ProductDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(p => p.Id == newProduct.Id);
                return Result<ProductDTO>.Success(data);
            }
        }
    }
}
