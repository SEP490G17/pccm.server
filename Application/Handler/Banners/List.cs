using Application.Core;
using MediatR;
using Application.DTOs;
using AutoMapper;
using Application.SpecParams;
using Application.Interfaces;
using Domain.Entity;
using Application.SpecParams.BannerSpec;
using Persistence;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Banners
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<BannerDto>>>
        {
            public BaseSpecParam BaseSpecParam { get; set; }
        }
        public class Handler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<Query, Result<Pagination<BannerDto>>>
        {
            public async Task<Result<Pagination<BannerDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // lấy các query param từ query
                var queryParams = request.BaseSpecParam;
                // tạo đặc tả cho query banner bao gồm cả phân trang
                var spec = new BannersSpecification(queryParams);
                // tạo đặc tả cho việc lấy tổng bản ghi của banner theo đặc tả không gồm phân trang
                var countSpec = new BannersCountSpecification(queryParams);
                // tổng bản ghi dựa trên đặc tả
                var totalItem = await _unitOfWork.Repository<Banner>().CountAsync(countSpec, cancellationToken);
                // danh sách các đặc tả
                var data = await _unitOfWork.Repository<Banner>().QueryList(spec)
                .ProjectTo<BannerDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                // map sang dto
                var result = new Pagination<BannerDto>(queryParams.PageSize, totalItem, data);
                return Result<Pagination<BannerDto>>.Success(result);
            }
        }
    }
}