using Application.Core;
using Application.DTOs;
using Application.Handler.StaffPositions;
using Application.Interfaces;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Services
{
    public class Create
    {
        public class Command : IRequest<Result<List<Service>>>
        {
            public ServiceInputDto Service { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Service).SetValidator(new ServiceInputDTOValidator());
            }
        }
        public class Handler(DataContext _context, IMapper _mapper, IMediator _mediator, IUserAccessor _userAccessor) : IRequestHandler<Command, Result<List<Service>>>
        {

            public async Task<Result<List<Service>>> Handle(Command request, CancellationToken cancellationToken)
            {
                ServiceInputDto serviceDtos = request.Service;

                List<int> clusterIds = await _mediator.Send(new GetCurrentStaffCluster.Query(), cancellationToken);

                if (clusterIds != null)
                {
                    var check = serviceDtos.CourtClusterId.Any(x => !clusterIds.Contains(x));
                    if (check)
                    {
                        return Result<List<Service>>.Failure("Bạn không có quyền thực hiện quyền này");
                    }
                }
                List<Service> services = [];
                List<string> existingCourtClusters = [];

                foreach (var courtClusterId in serviceDtos.CourtClusterId)
                {
                    var existingService = await _context.Services
                        .FirstOrDefaultAsync(x => x.ServiceName == serviceDtos.ServiceName
                        && x.CourtClusterId == courtClusterId
                        && x.DeletedAt == null
                        && x.DeletedBy == null
                        , cancellationToken);

                    if (existingService != null)
                    {
                        var courtClusterName = (await _context.CourtClusters.FirstOrDefaultAsync(x => x.Id == courtClusterId, cancellationToken))?.CourtClusterName;
                        if (courtClusterName != null)
                        {
                            existingCourtClusters.Add(courtClusterName);
                        }
                        continue;
                    }

                    Service service = new()
                    {
                        CourtClusterId = courtClusterId,
                        Description = serviceDtos.Description,
                        Price = serviceDtos.Price,
                        ServiceName = serviceDtos.ServiceName,
                        CourtCluster = await _context.CourtClusters.FirstOrDefaultAsync(x => x.Id == courtClusterId)
                    };

                    services.Add(service);
                }

                if (existingCourtClusters.Any())
                {
                    string message = $"Dịch vụ '{serviceDtos.ServiceName}' đã tồn tại ở {string.Join(", ", existingCourtClusters)}";
                    return Result<List<Service>>.Failure(message);
                }

                foreach (var service in services)
                {
                    await _context.Services.AddAsync(service, cancellationToken);
                }
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<List<Service>>.Failure("Fail to create service");

                foreach (var service in services)
                {
                    var serviceLog = _mapper.Map<ServiceLog>(service);
                    serviceLog.Id = 0;
                    serviceLog.CreatedAt = DateTime.Now;
                    serviceLog.CreatedBy = _userAccessor.GetUserName();
                    serviceLog.LogType = Domain.Enum.LogType.Create;
                    serviceLog.CourtCluster = await _context.CourtClusters.FirstOrDefaultAsync(c => c.Id == service.CourtClusterId);
                    serviceLog.Description = $"đã thêm dịch vụ {service.ServiceName}";
                    await _context.ServiceLogs.AddAsync(serviceLog, cancellationToken);
                }
                var result1 = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result1) return Result<List<Service>>.Failure("Fail to create servicelog");

                return Result<List<Service>>.Success(services);
            }
        }
    }
}