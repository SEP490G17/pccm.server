using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Banners
{
    public class Details
    {
        public class Query : IRequest<Result<BannerDto>> {
            public int Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<BannerDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<BannerDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var banner = await _context.Banners
                    .ProjectTo<BannerDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x=> x.Id == request.Id);
                if (banner is null)
                    return Result<BannerDto>.Failure("Banner not found");
                return Result<BannerDto>.Success(banner);
            }
        }
    }
}
