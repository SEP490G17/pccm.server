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
    public class Edit
    {
        public class Command : IRequest<Result<ServiceDto>>
        {
            public ServiceInputDTO Service { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Service).SetValidator(new ServiceValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<ServiceDto>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<ServiceDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var service = _mapper.Map<Service>(request.Service);
                var serviceExist = await _context.Services.Include(s => s.CourtCluster).FirstOrDefaultAsync(s => s.Id == request.Service.Id);
                if (serviceExist == null)
                    return Result<ServiceDto>.Failure("Fail to edit service");
                service.CourtCluster = serviceExist.CourtCluster;
                _mapper.Map(request.Service, serviceExist);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<ServiceDto>.Failure("Fail to edit service");
                var updateService = _context.Entry(service).Entity;
                var response = _mapper.Map<ServiceDto>(serviceExist);
                return Result<ServiceDto>.Success(response);
            }
        }
    }
}
