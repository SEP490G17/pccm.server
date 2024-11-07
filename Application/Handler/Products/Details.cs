using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Products
{
    public class Details
    {
        public class Query : IRequest<Result<ProductDto.ProductDetails>>
        {
            public int Id { get; set; }
        }
        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<ProductDto.ProductDetails>>
        {
            public async Task<Result<ProductDto.ProductDetails>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = request.Id;

                var result = await _unitOfWork.Repository<Product>().QueryList(null).ProjectTo<ProductDto.ProductDetails>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(p => p.Id == query);

                return Result<ProductDto.ProductDetails>.Success(result);
            }
        }
    }
}