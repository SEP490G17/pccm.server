using Application.Core;
using Application.Handler.StaffPositions;
using Application.Interfaces;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Services
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }

        public class Handler(DataContext _context, IMapper _mapper, IUserAccessor _userAccessor, IMediator _mediator) : IRequestHandler<Command, Result<Unit>>
        {

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var service = await _context.Services.FindAsync(request.Id);
                if (service is null || service.CourtClusterId is null) return null;
                List<int> clusterIds = await _mediator.Send(new GetCurrentStaffCluster.Query(), cancellationToken);
                if (clusterIds != null && !clusterIds.Contains((int)service.CourtClusterId))
                {
                    return Result<Unit>.Failure("Bạn không có quyền thực hiện quyền này");
                }
                service.DeletedAt = DateTime.Now;
                service.DeletedBy = _userAccessor.GetUserName();
                _context.Services.Update(service);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Unit>.Failure("Failed to delete the service.");
                var serviceLog = _mapper.Map<ServiceLog>(service);
                serviceLog.Id = 0;
                serviceLog.CreatedAt = DateTime.Now;
                serviceLog.CreatedBy = _userAccessor.GetUserName();
                serviceLog.LogType = Domain.Enum.LogType.Delete;
                serviceLog.CourtCluster = await _context.CourtClusters.FirstOrDefaultAsync(c => c.Id == service.CourtClusterId);
                serviceLog.Description = $"đã xóa dịch vụ {service.ServiceName}";
                await _context.ServiceLogs.AddAsync(serviceLog,cancellationToken);
                var result1 = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result1) return Result<Unit>.Failure("Failed to log delete the service.");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
