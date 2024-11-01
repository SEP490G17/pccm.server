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
            public ServiceInputDto Service { get; set; }
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
                foreach (var courtClusterId in serviceDtos.CourtClusterId)
                {
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