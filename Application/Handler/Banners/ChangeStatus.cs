using Application.Core;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Persistence;


namespace Application.Handler.Banners
{
    public class ChangeStatus
    {
        public class Command : IRequest<Result<Banner>>
        {
            public int Id { get; set; }
            public int status { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Banner>>
        {

            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Banner>> Handle(Command request, CancellationToken cancellationToken)
            {
                var banner = await _context.Banners.FindAsync(request.Id);

                if (banner == null)
                {
                    return Result<Banner>.Failure("Banner not found");
                }

                if (banner.Status != (BannerStatus)request.status)
                {
                    banner.Status = (request.status == 1) ? BannerStatus.Display : BannerStatus.Hidden;
                }

                try
                {
                    await _context.SaveChangesAsync(cancellationToken) ;
                    // if (!result)
                    // {
                    //     return Result<Banner>.Failure("Failed to change status banner");
                    // }

                    return Result<Banner>.Success(banner);
                }
                catch (Exception ex)
                {
                    return Result<Banner>.Failure($"Error: {ex.Message}");
                }
            }
        }
    }
}
