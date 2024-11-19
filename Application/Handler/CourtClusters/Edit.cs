using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtClusters
{
    public class Edit
    {
        public class Command : IRequest<Result<CourtCluster>>
        {
            public CourtClustersInputDto courtCluster { get; set; }
            public int id { get; set; }
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
                var existingCourtCluster = await _context.CourtClusters.FirstOrDefaultAsync(x => x.Id == request.id, cancellationToken);

                if (existingCourtCluster == null)
                {
                    return Result<CourtCluster>.Failure("Court cluster not found.");
                }

                _mapper.Map(request.courtCluster, existingCourtCluster);

                // Update the entity in the database
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<CourtCluster>.Failure("Failed to edit court cluster. Changes were not saved.");
                }

                return Result<CourtCluster>.Success(_context.Entry(existingCourtCluster).Entity);
            }
        }
    }
}