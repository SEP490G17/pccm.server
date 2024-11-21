using Application.Core;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Courts
{
    public class Detail
    {
        public class Query : IRequest<Result<Court>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Court>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Court>> Handle(Query request, CancellationToken cancellationToken)
            {
                var court = await _context.Courts
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.DeleteAt == null);

                if (court is null)
                    return Result<Court>.Failure("Court not found");
                return Result<Court>.Success(court);
            }
        }
    }
}