﻿using Application.Core;
using Application.DTOs;
using Application.Handler.StaffPositions;
using Application.Interfaces;
using Application.SpecParams.ProductSpecification;
using Application.SpecParams.ServiceSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Services
{
    public class ListServiceLog
    {
        public class Query : IRequest<Result<Pagination<ServiceLogDto>>>
        {
            public ServiceLogSpecParams BaseSpecParam { get; set; }
        }
        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork, IMediator mediator) : IRequestHandler<Query, Result<Pagination<ServiceLogDto>>>
        {

            public async Task<Result<Pagination<ServiceLogDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecParam;
                List<int> courtClusterId = await mediator.Send(new GetCurrentStaffCluster.Query(), cancellationToken);
                var spec = new ServicesLogSpecification(querySpec, courtClusterId);
                var specCount = new ServicesLogCountSpecification(querySpec, courtClusterId);

                var totalElement = await _unitOfWork.Repository<ServiceLog>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<ServiceLog>().QueryList(spec)
                .ProjectTo<ServiceLogDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

                return Result<Pagination<ServiceLogDto>>.Success(new Pagination<ServiceLogDto>(querySpec.PageSize, totalElement, data));
            }
        }
    }
}
