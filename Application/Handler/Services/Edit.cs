using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Services
{
    public class Edit
    {
        public class Command : IRequest<Result<Service>>
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
        public class Handler : IRequestHandler<Command, Result<Service>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Service>> Handle(Command request, CancellationToken cancellationToken)
            {
                var service = _mapper.Map<Service>(request.Service);
                var serviceExist = await _context.Services.FindAsync(request.Service.Id);
                _mapper.Map(service, serviceExist);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Service>.Failure("Faild to edit service");
                return Result<Service>.Success(_context.Entry(service).Entity);
            }
        }
    }
}
