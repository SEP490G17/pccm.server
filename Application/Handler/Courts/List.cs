using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.CourtCountSpecification;
using Application.SpecParams.CourtSpecification;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Courts
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<Court>>>
        {
            public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<Court>>>
        {
            public async Task<Result<Pagination<Court>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecWithFilterParam;

                var spec = new CourtSpecification(querySpec);
                var specCount = new CourtCountSpecification(querySpec);

                var totalElement = await _unitOfWork.Repository<Court>().CountAsync(specCount, cancellationToken);
                var data = await _unitOfWork.Repository<Court>().QueryList(spec).ToListAsync(cancellationToken);

                return Result<Pagination<Court>>.Success(new Pagination<Court>(querySpec.PageSize, totalElement, data));
            }
        }

    }
}