using Application.Core;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Courts
{
    public class Search
    {
        public class Query : IRequest<Result<List<Court>>>
        {
            public string value { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Court>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<Court>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var courts = await _context.Courts.Where(x => x.CourtName.Contains(request.value)).ToListAsync(cancellationToken);
                return Result<List<Court>>.Success(courts);
            }
        }
    }
}