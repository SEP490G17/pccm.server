using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Statistics
{
    public class SaveRevenue
    {
        public class Command : IRequest<Result<Revenue>>
        {
            public SaveRevenueDto revenue { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Revenue>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Revenue>> Handle(Command request, CancellationToken cancellationToken)
            {
                var revenueEntity = new Revenue
                {
                    courtClusterId = request.revenue.courtClusterId,
                    revenueAt = request.revenue.Date,
                    BookingDetail = JsonSerializer.Serialize(request.revenue.BookingDetails),
                    ProductDetail = JsonSerializer.Serialize(request.revenue.OrderProductDetails),
                    ServiceDetail = JsonSerializer.Serialize(request.revenue.OrderServiceDetails),
                    ExpenseDetail = JsonSerializer.Serialize(request.revenue.ExpenseDetails)
                };
                await _context.Revenues.AddAsync(revenueEntity);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Revenue>.Failure("Fail to create or update expense");
                return Result<Revenue>.Success(revenueEntity);
            }
        }
    }
}