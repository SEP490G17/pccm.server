﻿using Application.Core;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Persistence;

namespace Application.Handler.Banners
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
            public string userName { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var banner = await _context.Banners.FindAsync(request.Id);
                if (banner is null) return null;
                banner.DeletedBy = request.userName;

                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                banner.DeletedAt = vietnamTime;

                var bannerLog = _mapper.Map<BannerLog>(banner);
                if (bannerLog == null)
                {
                    return Result<Unit>.Failure("Failed to create banner log.");
                }

                bannerLog.Id = 0;
                bannerLog.BannerId = banner.Id;
                bannerLog.CreatedBy = request.userName;
                bannerLog.CreatedAt = vietnamTime;
                bannerLog.Description = "Banner has been successfully removed";
                bannerLog.LogType = LogType.Delete;


                await _context.BannerLogs.AddAsync(bannerLog, cancellationToken);

                // Save changes to the context
                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the banner.");
            }
        }
    }
}
