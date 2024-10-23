using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.StaffPositions
{
    public class List
    {
        public class Query : IRequest<Result<List<StaffPosition>>> { }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<List<StaffPosition>>>
        {

            public async Task<Result<List<StaffPosition>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tasks = await _unitOfWork.Repository<StaffPosition>().QueryList(null).ToListAsync(cancellationToken);
                return Result<List<StaffPosition>>.Success(tasks);
            }
        }
    }
}