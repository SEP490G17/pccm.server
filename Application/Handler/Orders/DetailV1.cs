using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Orders
{
    public class DetailV1
    {
        public class Query : IRequest<Result<OrderDetailsResponse>>
        {
            public int Id { get; set; }

        }

        public class Handler : IRequestHandler<Query, Result<OrderDetailsResponse>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }
            public async Task<Result<OrderDetailsResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var order = await _context.Orders
                .ProjectTo<OrderDetailsResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (order is null) return Result<OrderDetailsResponse>.Failure("Order not found");
                return Result<OrderDetailsResponse>.Success(order);
            }
        }
    }
}