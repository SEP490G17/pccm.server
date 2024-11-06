using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.ProductSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Products
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<ProductDto>>>
        {
            public ProductSpecParams SpecParam { get; set; }
        }
        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<ProductDto>>>
        {
            public async Task<Result<Pagination<ProductDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.SpecParam;

                var spec = new ProductsSpecification(querySpec);
                var specCount = new ProductsCountSpecification(querySpec);

                var totalElement = await _unitOfWork.Repository<Product>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<Product>()
                                            .QueryList(spec)
                                            .Where(b => b.DeletedAt == null)
                                            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                                            .ToListAsync(cancellationToken);

                return Result<Pagination<ProductDto>>.Success(new Pagination<ProductDto>(querySpec.PageSize, totalElement, data));
            }
        }
    }
}