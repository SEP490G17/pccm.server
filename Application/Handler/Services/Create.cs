﻿using Application.Core;
using Application.DTOs;
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
            public string userName { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Service).SetValidator(new ServiceInputDTOValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<List<Service>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<List<Service>>> Handle(Command request, CancellationToken cancellationToken)
            {
                ServiceInputDto serviceDtos = request.Service;
                List<Service> services = new List<Service>();
                List<string> existingCourtClusters = new List<string>();

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

                    Service service = new Service()
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
                    serviceLog.CreatedBy = request.userName;
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