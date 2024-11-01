using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class Create
    {
        public class Command : IRequest<Result<CourtCluster>>
        {
            public CourtClustersInputDto CourtCluster { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.CourtCluster).SetValidator(new CourtClusterValidator());
            }
        }

        public class Handler(DataContext _context,IMapper _mapper) : IRequestHandler<Command, Result<CourtCluster>>
        {
          
            public async Task<Result<CourtCluster>> Handle(Command request, CancellationToken cancellationToken)
            {
                var courtCluster = _mapper.Map<CourtCluster>(request.CourtCluster);
                await _context.AddAsync(courtCluster, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<CourtCluster>.Failure("Fail to create court cluster.");
                var newCourtCluster = _context.Entry(courtCluster).Entity;
                return Result<CourtCluster>.Success(newCourtCluster);
            }
        }
    }
}