using Application.Core;
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
            public ServiceInputDTO Service { get; set; }
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
                ServiceInputDTO serviceDtos = request.Service;
                List<Service> services = new List<Service>();
                List<string> existingCourtClusters = new List<string>();

                foreach (var courtClusterId in serviceDtos.CourtClusterId)
                {
                    // Kiểm tra xem dịch vụ có tồn tại với ServiceName và CourtClusterId hay không
                    var existingService = await _context.Services
                        .FirstOrDefaultAsync(x => x.ServiceName == serviceDtos.ServiceName && x.CourtClusterId == courtClusterId, cancellationToken);

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

                // Nếu có các CourtCluster đã tồn tại, trả về thông báo lỗi với tên của chúng
                if (existingCourtClusters.Any())
                {
                    string message = $"Dịch vụ '{serviceDtos.ServiceName}' đã tồn tại trong các CourtCluster: {string.Join(", ", existingCourtClusters)}";
                    return Result<List<Service>>.Failure(message);
                }

                foreach (var service in services)
                {
                    await _context.AddAsync(service, cancellationToken);
                    var newService = _context.Entry(service).Entity;
                }
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<List<Service>>.Failure("Fail to create service");

                return Result<List<Service>>.Success(services);
            }
        }
    }
}