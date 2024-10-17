using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Services
{
    public class Create
    {
        public class Command : IRequest<Result<Service>>
        {
            public ServiceDto Service { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Service).SetValidator(new ServiceValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<Service>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<Service>> Handle(Command request, CancellationToken cancellationToken)
            {
                var service = _mapper.Map<Service>(request.Service);
                await _context.AddAsync(service, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Service>.Failure("Fail to create service");
                var newService = _context.Entry(service).Entity;
                return Result<Service>.Success(newService);
            }
        }
    }
}
