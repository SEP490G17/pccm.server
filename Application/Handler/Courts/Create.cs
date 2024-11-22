using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Courts
{
    public class Create
    {
        public class Command : IRequest<Result<CourtOfClusterDto>>
        {
            public CourtCreateDto Court { get; set; }
        }

        public class Handler(DataContext _context, IMapper mapper) : IRequestHandler<Command, Result<CourtOfClusterDto>>
        {
            public async Task<Result<CourtOfClusterDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var court = new Court()
                {
                    CourtClusterId = request.Court.CourtClusterId,
                    CourtName = request.Court.CourtName
                };

                var courtPrices = mapper.Map<List<CourtPrice>>(request.Court.CourtPrice);

                court.CourtPrices = courtPrices;
                await _context.AddAsync(court, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<CourtOfClusterDto>.Failure("Fail to create court");
                var newCourt = _context.Entry(court).Entity;
                var response = await _context.Courts
                .ProjectTo<CourtOfClusterDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.CourtId == newCourt.Id, cancellationToken);
                return Result<CourtOfClusterDto>.Success(response);
            }
        }
    }
}