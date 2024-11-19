using Application.Core;
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
                var service = await _context.Services.FindAsync(request.Id);
                if (service is null) return null;
                service.DeletedAt = DateTime.Now;
                service.DeletedBy = "Anonymous";
                _context.Services.Update(service);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit>.Failure("Failed to delete the service.");
                var serviceLog = _mapper.Map<ServiceLog>(service);
                serviceLog.Id = 0;
                serviceLog.CreatedAt = DateTime.Now;
                serviceLog.CreatedBy = request.userName;
                serviceLog.LogType = Domain.Enum.LogType.Delete;
                serviceLog.CourtCluster = await _context.CourtClusters.FirstOrDefaultAsync(c => c.Id == service.CourtClusterId);
                serviceLog.Description = $"đã xóa dịch vụ {service.ServiceName}";
                await _context.ServiceLogs.AddAsync(serviceLog);
                var result1 = await _context.SaveChangesAsync() > 0;
                if (!result1) return Result<Unit>.Failure("Failed to log delete the service.");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
