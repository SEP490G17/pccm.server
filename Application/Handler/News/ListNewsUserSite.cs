using Application.Core;
using MediatR;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.News
{
    public class ListNewsUserSite
    {
        public class Query : IRequest<Result<List<NewsBlogDto>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<NewsBlogDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<NewsBlogDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var news = await _context.NewsBlogs.Where(b => b.Status == Domain.Enum.BannerStatus.Display).ProjectTo<NewsBlogDto>(_mapper.ConfigurationProvider).ToListAsync();
                return Result<List<NewsBlogDto>>.Success(news);
            }
        }
    }
}
