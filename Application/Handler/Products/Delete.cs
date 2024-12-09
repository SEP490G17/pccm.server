using Application.Core;
using Application.Handler.StaffPositions;
using Application.Interfaces;
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
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IMediator mediator, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mediator = mediator;
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {

                var deleteProduct = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.CourtCluster)
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.DeletedAt == null,cancellationToken);

                if (deleteProduct is null) return Result<Unit>.Failure("Sản phẩm không tồn tại hoặc đã bị xoá");
                List<int> courtClusterId = await _mediator.Send(new GetCurrentStaffCluster.Query(), cancellationToken);

                if (courtClusterId != null && !courtClusterId.Contains((int)deleteProduct.CourtClusterId))
                {
                    return Result<Unit>.Failure("Bạn không có quyền thực hiện quyền này");
                }

                deleteProduct.DeletedAt = DateTime.Now;

                var productLog = _mapper.Map<ProductLog>(deleteProduct);
                if (productLog == null)
                {
                    return Result<Unit>.Failure("Failed to create product log.");
                }

                productLog.Id = 0;
                productLog.CreatedBy = _userAccessor.GetUserName();
                productLog.CreatedAt = DateTime.Now;
                productLog.Description = $"đã xóa {(int)deleteProduct.Quantity} {deleteProduct.ProductName}";
                productLog.LogType = LogType.Delete;

                await _context.ProductLogs.AddAsync(productLog, cancellationToken);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the product.");
            }
        }
    }
}
