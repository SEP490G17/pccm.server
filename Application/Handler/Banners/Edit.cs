using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Banners
{
    public class Edit
    {
        public class Command : IRequest<Result<BannerInputDto>>
        {
            public BannerInputDto Banner { get; set; }
            public string userName { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Banner).SetValidator(new BannerValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<BannerInputDto>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;

            }
            public async Task<Result<BannerInputDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                var banner = await _context.Banners.FindAsync(request.Banner.Id);
                if (banner == null)
                    return Result<BannerInputDto>.Failure("Banner not found");

                var bannerLog = _mapper.Map<BannerLog>(banner);
                if (bannerLog == null)
                {
                    return Result<BannerInputDto>.Failure("Failed to create banner log.");
                }

                bannerLog.Id = 0;
                bannerLog.BannerId = banner.Id;
                bannerLog.CreatedBy = request.userName;
                bannerLog.Description = "Banner has been updated successfully";
                bannerLog.CreatedAt = vietnamTime;
                bannerLog.LogType = LogType.Update;



                await _context.BannerLogs.AddAsync(bannerLog, cancellationToken);

                _mapper.Map(request.Banner, banner);

                banner.UpdatedBy = request.userName;

                banner.UpdatedAt = vietnamTime;

                // Save changes
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<BannerInputDto>.Failure("Failed to edit banner");

                var bannerDto = _mapper.Map<BannerInputDto>(banner);
                return Result<BannerInputDto>.Success(bannerDto);
            }
        }
    }
}
