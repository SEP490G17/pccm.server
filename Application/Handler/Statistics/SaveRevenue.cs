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
        public class Command : IRequest<Result<Unit>>
        {
            public ClusterStatisticsDto revenue { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var data = request.revenue;
                var revenueEntity = new Revenue
                {
                    courtClusterId = 1,
                    revenueAt = DateTime.Now,
                    BookingDetail = JsonSerializer.Serialize(data.BookingDetails),
                    ProductDetail = JsonSerializer.Serialize(data.OrderProductDetails),
                    ServiceDetail = JsonSerializer.Serialize(data.OrderServiceDetails),
                    ExpenseDetail = JsonSerializer.Serialize(data.ExpenseDetails)
                };
                return Result<Unit>.Failure("Failed to save revenue");
            }
        }
    }
}