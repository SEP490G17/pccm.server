using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtClusters
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
        private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
            this._userAccessor = userAccessor;
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var courtCluster = await _context.CourtClusters.FindAsync(request.Id, cancellationToken);
                if (courtCluster is null) return Result<Unit>.Failure("Cụm sân không tồn tại.");
                courtCluster.DeleteAt = DateTime.Now;
                courtCluster.DeleteBy = await _context.Users.FirstOrDefaultAsync(x=>x.UserName.Equals(_userAccessor.GetUserName()));
                _context.CourtClusters.Update(courtCluster);
                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the court cluster.");
            }
        }
    }
}