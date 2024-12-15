using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Courts
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var court = await _context.Courts.FindAsync(request.Id);
                if (court is null){
                    return Result<Unit>.Failure("Không tìm thấy sân.");
                }
                var courtCluster = await _context.Courts.Where(c => c.CourtClusterId == court.CourtClusterId && c.DeleteAt == null).ToListAsync(cancellationToken);

                if (courtCluster.Count <= 1)
                {
                    return Result<Unit>.Failure("Cụm sân cần ít nhất một sân.");
                }
                court.DeleteAt = DateTime.UtcNow;
                _context.Courts.Update(court);
                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the court.");
            }
        }
    }
}