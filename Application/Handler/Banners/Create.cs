using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Security.Claims;

namespace Application.Handler.Banners
{
    public class Create
    {
        public class Command : IRequest<Result<BannerInputDto>>
        {
            public BannerInputDto Banner { get; set; }
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
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<BannerInputDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                string userName = _userAccessor.GetUserName();
                if (request.Banner == null)
                {
                    return Result<BannerInputDto>.Failure("Banner data is required.");
                }

                var banner = _mapper.Map<Banner>(request.Banner);
                if (!string.IsNullOrEmpty(userName))
                {
                    banner.CreatedBy = userName;
                }

                await _context.Banners.AddAsync(banner, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result)
                    return Result<BannerInputDto>.Failure("Failed to create banner");

                var bannerLog = _mapper.Map<BannerLog>(banner);
                if (bannerLog == null)
                {
                    return Result<BannerInputDto>.Failure("Failed to create banner log.");
                }

                bannerLog.Id = 0;
                bannerLog.BannerId = banner.Id; 
                bannerLog.CreatedBy = userName;
                bannerLog.Description = "Banner created successfully";
                bannerLog.LogType = LogType.Create;


                await _context.BannerLogs.AddAsync(bannerLog, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);


                var data = await _context.Banners
                    .Where(b => b.Id == banner.Id)
                    .ProjectTo<BannerInputDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

                return Result<BannerInputDto>.Success(data);
            }
        }
    }
}
