using Application.Core;
using MediatR;
using Persistence;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Banners
{
    public class List
    {
        public class Query : IRequest<Result<List<BannerDto>>>{}
        public class Handler : IRequestHandler<Query, Result<List<BannerDto>>>{
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<List<BannerDto>>> Handle(Query request, CancellationToken cancellationToken){
                var bannerGroup = await _context.Banners
                .ProjectTo<BannerDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

                return Result<List<BannerDto>>.Success(bannerGroup);
            }
        }
    }
}