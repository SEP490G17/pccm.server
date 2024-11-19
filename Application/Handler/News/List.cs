using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.EventSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.News
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<NewsBlogDto>>>
        {
            public BaseSpecWithFilterParam BaseSpecParam { get; set; }
        }

        public class Handler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<Query, Result<Pagination<NewsBlogDto>>>
        {
            public async Task<Result<Pagination<NewsBlogDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecParam;
                var spec = new EventsSpecification(querySpec);
                var specCount = new EventsCountSpecification(querySpec);
                var totalItem = await _unitOfWork.Repository<NewsBlog>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<NewsBlog>().QueryList(spec)
                .ProjectTo<NewsBlogDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
                return Result<Pagination<NewsBlogDto>>.Success(new Pagination<NewsBlogDto>(querySpec.PageSize, totalItem, data));
            }
        }

    }
}