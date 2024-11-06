﻿using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Persistence;
using System.Reflection;

namespace Application.Handler.Products
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IUserAccessor userAccessor,IMapper mapper)
            {
                _context = context;
                _userAccessor = userAccessor;
                _mapper = mapper;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                string userName = _userAccessor.GetUserName();

                var deleteProduct = await _context.Products.FindAsync(request.Id);
                if (deleteProduct is null) return null;

                if (userName != null)
                {
                    deleteProduct.DeletedBy = userName;
                }
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                deleteProduct.DeletedAt = vietnamTime;

                var productLog = _mapper.Map<ProductLog>(deleteProduct);
                if (productLog == null)
                {
                    return Result<Unit>.Failure("Failed to create product log.");
                }

                productLog.Id = 0;
                productLog.ProductId = deleteProduct.Id;
                productLog.CreatedBy = userName;
                productLog.CreatedAt = vietnamTime;
                productLog.Description = "Delete Product";

                await _context.ProductLogs.AddAsync(productLog, cancellationToken);

                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the product.");
            }
        }
    }
}
