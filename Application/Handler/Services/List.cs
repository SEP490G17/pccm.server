using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.ServiceSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Services
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<ServiceDto>>>
        {
            public BaseSpecParam BaseSpecParam { get; set; }
        }
        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<ServiceDto>>>
        {

            public async Task<Result<Pagination<ServiceDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecParam;

                var spec = new ServicesSpecification(querySpec);
                var specCount = new ServicesCountSpecification(querySpec);

                var totalElement = await _unitOfWork.Repository<Service>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<Service>().QueryList(spec)
                .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

                return Result<Pagination<ServiceDto>>.Success(new Pagination<ServiceDto>(querySpec.PageSize, totalElement, data));
            }
        }
    }
}
