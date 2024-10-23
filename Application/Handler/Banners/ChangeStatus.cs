using Application.Core;
using Domain.Enum;
using MediatR;
using Persistence;


namespace Application.Handler.Banners
{
    public class ChangeStatus
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
            public int status { get; set; }
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
                var banner = await _context.Banners.FindAsync(request.Id);
                if (request.status == 1)
                {
                    banner.Status = BannerStatus.Display;
                }
                else
                {
                    banner.Status = BannerStatus.Hidden;
                }
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit>.Failure("Faild to change status banner");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
