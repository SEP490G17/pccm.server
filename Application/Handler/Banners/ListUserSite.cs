using Application.Core;
using MediatR;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Banners
{
    public class ListUserSite
    {
        public class Query : IRequest<Result<List<BannerDto>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<BannerDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<BannerDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var banners = await _context.Banners.Where(b => b.Status == Domain.Enum.BannerStatus.Display).ProjectTo<BannerDto>(_mapper.ConfigurationProvider).ToListAsync();
                return Result<List<BannerDto>>.Success(banners);
            }
        }
    }
}
