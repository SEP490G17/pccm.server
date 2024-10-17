using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.EventSpecification;
using AutoMapper;
using Domain.Entity;
using MediatR;

namespace Application.Handler.News
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<NewsBlogDTO>>>
        {
            public BaseSpecParam BaseSpecParam { get; set; }
        }

        public class Handler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<Query, Result<Pagination<NewsBlogDTO>>>
        {
            public async Task<Result<Pagination<NewsBlogDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecParam;
                var spec = new EventsSpecification(querySpec);
                var specCount = new EventsCountSpecification(querySpec);
                var totalItem = await _unitOfWork.Repository<NewsBlog>().CountAsync(specCount, cancellationToken);
                var events = await _unitOfWork.Repository<NewsBlog>().ListAsync(spec, cancellationToken);
                var data = _mapper.Map<IReadOnlyList<NewsBlog>, IReadOnlyList<NewsBlogDTO>>(events);
                return Result<Pagination<NewsBlogDTO>>.Success(new Pagination<NewsBlogDTO>(querySpec.PageSize, totalItem, data));
            }
        }

    }
}