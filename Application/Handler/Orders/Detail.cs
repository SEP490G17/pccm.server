using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Orders
{
    public class Detail
    {
          public class Query : IRequest<Result<Order>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Order>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                this._context = context;
            }
            public async Task<Result<Order>> Handle(Query request, CancellationToken cancellationToken)
            {
                var order = await _context.Orders.Include(o => o.Staff).Include(o => o.Booking).FirstOrDefaultAsync(x => x.Id == request.Id);
                
                if (order is null) return Result<Order>.Failure("Order not found");
                return Result<Order>.Success(order);
            }
        }
    }
}