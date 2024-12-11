using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Notifications
{
    public class UpdateReadAtNotification
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int NotiId { get; set; }
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

                var noti = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == request.NotiId, cancellationToken);
                if (noti.ReadAt == null)
                {
                    noti.ReadAt = DateTime.Now;
                    _context.Update(noti);
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result) return Result<Unit>.Failure("Cập nhật trạng thái thất bại");

                }
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}