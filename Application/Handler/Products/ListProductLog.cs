using Application.Core;
using Application.DTOs;
using Application.Handler.StaffPositions;
using Application.Interfaces;
using Application.SpecParams.ProductSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Products
{
    public class ListProductLog
    {
        public class Query : IRequest<Result<Pagination<ProductLogDto>>>
        {
            public ProductLogSpecParams SpecParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork,IMediator mediator) : IRequestHandler<Query, Result<Pagination<ProductLogDto>>>
        {
            public async Task<Result<Pagination<ProductLogDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.SpecParam;
                List<int> courtClusterId = await mediator.Send(new GetCurrentStaffCluster.Query(),cancellationToken);
                var spec = new ProductsLogSpecification(querySpec, courtClusterId);
                var specCount = new ProductsLogCountSpecification(querySpec, courtClusterId);

                var totalElement = await _unitOfWork.Repository<ProductLog>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<ProductLog>()
                                            .QueryList(spec)
                                            .ProjectTo<ProductLogDto>(_mapper.ConfigurationProvider)
                                            .ToListAsync(cancellationToken);

                return Result<Pagination<ProductLogDto>>.Success(new Pagination<ProductLogDto>(querySpec.PageSize, totalElement, data));
            }
        }
        // public class Handler() : IRequestHandler<Query, Result<Pagination<ProductDto>>>
        // {
        //     public async Task<Result<Pagination<ProductDto>>> Handle(Query request, CancellationToken cancellationToken)
        //     {
        //         return Result<Pagination<ProductDto>>.Success(new Pagination<ProductDto>(0, 0, new List<ProductDto>()));
        //     }
        // }
    }
}