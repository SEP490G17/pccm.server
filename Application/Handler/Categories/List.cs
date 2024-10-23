

using Application.Core;
using Application.Interfaces;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Categories
{
    public class List
    {
        public class Query : IRequest<Result<IReadOnlyList<Category>>> { }

        public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<Category>>>
        {
            public async Task<Result<IReadOnlyList<Category>>> Handle(Query request, CancellationToken cancellationToken)
            {

                var activityGroup = await unitOfWork.Repository<Category>().ListAllAsync();
                return Result<IReadOnlyList<Category>>.Success(activityGroup);
            }
        }

    }
}