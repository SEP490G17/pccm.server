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
    public class Detail
    {
        public class Query : IRequest<Result<ServiceDto>>
        {
            public int Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<ServiceDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<ServiceDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var service = await _context.Services
                    .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (service is null)
                    return Result<ServiceDto>.Failure("Service not found");
                return Result<ServiceDto>.Success(service);
            }
        }
    }
}
