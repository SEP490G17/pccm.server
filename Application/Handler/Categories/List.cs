

using Application.Core;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Categories
{
    public class List
    {
        public class Query : IRequest<Result<List<Category>>> { }

        public class Handler : IRequestHandler<Query, Result<List<Category>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<Category>>> Handle(Query request, CancellationToken cancellationToken)
            {

                var activityGroup = await _context.Categories.ToListAsync(cancellationToken);
                return Result<List<Category>>.Success(activityGroup);
            }
        }

    }
}