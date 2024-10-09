using Application.Core;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Events
{
    public class List
    {
        public class Query : IRequest<Result<List<Event>>> { }

        public class Handler : IRequestHandler<Query, Result<List<Event>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<Event>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activityGroup = await _context.Events.ToListAsync(cancellationToken);
                return Result<List<Event>>.Success(activityGroup);
            }
        }

    }
}