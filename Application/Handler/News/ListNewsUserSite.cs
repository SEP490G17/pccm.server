using Application.Core;
using MediatR;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.SpecParams.NewsSpecification;
using Application.Interfaces;
using Domain.Entity;

namespace Application.Handler.News
{
    public class ListNewsUserSite
    {
        public class Query : IRequest<Result<Pagination<NewsBlogDto>>>
        {
            public NewsSpecParams NewsSpecParams { get; set; }
        }

        public class Handler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<Query, Result<Pagination<NewsBlogDto>>>
        {

            public async Task<Result<Pagination<NewsBlogDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = new NewsSpecification(request.NewsSpecParams);
                var countQuery = new NewsCountSpecification(request.NewsSpecParams);
                var news = await _unitOfWork.Repository<NewsBlog>().QueryList(query).ProjectTo<NewsBlogDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                var count = await _unitOfWork.Repository<NewsBlog>().CountAsync(countQuery, cancellationToken);
                return Result<Pagination<NewsBlogDto>>.Success(new Pagination<NewsBlogDto>
                {
                    Count = count,
                    Data = news,
                    PageSize = request.NewsSpecParams.PageSize,
                });
            }
        }
    }
}
