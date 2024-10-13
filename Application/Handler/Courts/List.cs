using Application.Core;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Courts
{
    public class List
    {
        public class Query : IRequest<Result<List<Court>>> { }

        public class Handler : IRequestHandler<Query, Result<List<Court>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<Court>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var court = await _context.Courts.ToListAsync(cancellationToken);
                return Result<List<Court>>.Success(court);
            }
        }

    }
}