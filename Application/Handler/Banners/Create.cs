using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Banners
{
    public class Create
    {
        public class Command : IRequest<Result<BannerDto>>
        {
            public Banner Banner { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Banner).SetValidator(new BannerValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<BannerDto>>
        {
            private readonly DataContext _context;

            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<BannerDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var banner = _mapper.Map<Banner>(request.Banner);
                await _context.AddAsync(banner, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<BannerDto>.Failure("Fail to create banner");
                var newBanner = _context.Entry(banner).Entity;
                var data = await _context.Banners.ProjectTo<BannerDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(p => p.Id == newBanner.Id);
                return Result<BannerDto>.Success(data);
            }
        }
    }
}
