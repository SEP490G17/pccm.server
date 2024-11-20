
using Application.Core;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Courts
{
    public class ToggleCourt
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
            public int Status { get; set; }
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
                var court = await _context.Courts.FirstOrDefaultAsync(x => x.Id == request.Id && x.DeleteAt == null);
                if (court is null)
                {
                    return Result<Unit>.Failure("Sân không tồn tại.");

                }
                court.Status = (CourtStatus)request.Status;
                _context.Courts.Update(court);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the court.");
            }
        }
    }
}