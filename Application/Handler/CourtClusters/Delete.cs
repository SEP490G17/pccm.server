using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
             public AppUser userApp { get; set; }
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
                var courtCluster = await _context.CourtClusters.FindAsync(request.Id);
                if (courtCluster is null) return Result<Unit>.Failure("Cụm sân không tồn tại.");
                courtCluster.DeleteAt = DateTime.Now;
                courtCluster.DeleteBy = request.userApp;
                _context.CourtClusters.Update(courtCluster);
                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the court cluster.");
            }
        }
    }
}