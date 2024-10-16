using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Services
{
    public class List
    {
        public class Query : IRequest<Result<List<ServiceDto>>> { }
        public class Handler : IRequestHandler<Query, Result<List<ServiceDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<List<ServiceDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var serviceGroup = await _context.Services
                .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

                return Result<List<ServiceDto>>.Success(serviceGroup);
            }
        }
    }
}
