using Application.Core;
using Application.DTOs;
using Application.Handler.Courts;
using AutoMapper;
using Domain;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class Edit
    {
        public class Command : IRequest<Result<CourtCluster>>
        {
            public CourtClustersInputDTO courtCluster { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.courtCluster).SetValidator(new CourtClusterValidator());

            }
        }
        public class Handler : IRequestHandler<Command, Result<CourtCluster>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<CourtCluster>> Handle(Command request, CancellationToken cancellationToken)
            {
                 var courtCluster = _mapper.Map<CourtCluster>(request.courtCluster);
                _context.CourtClusters.Update(courtCluster);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<CourtCluster>.Failure("Faild to edit court cluster.");
                return Result<CourtCluster>.Success(_context.Entry(courtCluster).Entity);
            }
        }
    }
}