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
        public class Query : IRequest<Result<Pagination<ProductDTO>>>
        {
            public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }
        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<ProductDTO>>>
        {

            public async Task<Result<Pagination<ProductDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecWithFilterParam;

                var spec = new ProductsSpecification(querySpec);
                var specCount = new ProductsCountSpecification(querySpec);

                var totalElement = await _unitOfWork.Repository<Product>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<Product>().QueryList(spec)
                .ProjectTo<ProductDTO>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

                return Result<Pagination<ProductDTO>>.Success(new Pagination<ProductDTO>(querySpec.PageSize, totalElement, data));
            }
        }
    }
}