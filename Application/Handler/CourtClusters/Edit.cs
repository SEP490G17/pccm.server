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
        public class Command : IRequest<Result<Unit>>
        {
            public int CourtClusterId { get; set; }
            public CourtClustersEditInput courtCluster { get; set; }
        }
        // public class CommandValidator : AbstractValidator<Command>
        // {
        //     public CommandValidator()
        //     {
        //         RuleFor(x => x.courtCluster).SetValidator(new CourtClusterValidator());

        //     }
        // }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var courtCluster = request.courtCluster;
                var check = await _context.CourtClusters.FirstOrDefaultAsync(c => c.Id == request.CourtClusterId, cancellationToken);
                if (check == null) return Result<Unit>.Failure("Cụm sân không tồn tại");
                var map = _mapper.Map(courtCluster, check);
                _context.CourtClusters.Update(map);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}