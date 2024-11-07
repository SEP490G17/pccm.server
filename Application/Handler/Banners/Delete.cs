using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Persistence;

namespace Application.Handler.Banners
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
            private readonly IMapper _mapper;

            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
            {
                _context = context;
                _userAccessor = userAccessor;
                _mapper = mapper;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                string userName = _userAccessor.GetUserName();
                var banner = await _context.Banners.FindAsync(request.Id);
                if (banner is null) return null;

                if (userName != null)
                {
                    banner.DeletedBy = userName;
                }

                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                banner.DeletedAt = vietnamTime;

                var bannerLog = _mapper.Map<BannerLog>(banner);
                if (bannerLog == null)
                {
                    return Result<Unit>.Failure("Failed to create banner log.");
                }

                bannerLog.Id = 0;
                bannerLog.BannerId = banner.Id;
                bannerLog.CreatedBy = userName;
                bannerLog.CreatedAt = vietnamTime;
                bannerLog.Description = "Delete Banner";


                await _context.BannerLogs.AddAsync(bannerLog, cancellationToken);

                // Save changes to the context
                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the banner.");
            }
        }
    }
}
