using Application.Core;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Products
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
            public string userName { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var deleteProduct = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.CourtCluster)
                .FirstOrDefaultAsync(p => p.Id == request.Id);
                if (deleteProduct is null) return null;

                deleteProduct.DeletedAt = DateTime.Now;

                var productLog = _mapper.Map<ProductLog>(deleteProduct);
                if (productLog == null)
                {
                    return Result<Unit>.Failure("Failed to create product log.");
                }

                productLog.Id = 0;
                productLog.CreatedBy = request.userName;
                productLog.CreatedAt = DateTime.Now;
                productLog.Description = $"đã xóa {(int)deleteProduct.Quantity} {deleteProduct.ProductName}";
                productLog.LogType = LogType.Delete;

                await _context.ProductLogs.AddAsync(productLog, cancellationToken);

                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the product.");
            }
        }
    }
}
