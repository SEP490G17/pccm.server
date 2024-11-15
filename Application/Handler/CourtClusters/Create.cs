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

        public class Handler(DataContext _context, IMapper _mapper) : IRequestHandler<Command, Result<CourtCluster>>
        {

            public async Task<Result<CourtCluster>> Handle(Command request, CancellationToken cancellationToken)
            {
                var courtClusterRes = request.CourtCluster;
                var courtRes = request.CourtCluster.CourtDetails;
                var courtCluster = _mapper.Map<CourtCluster>(courtClusterRes);

                courtRes.ForEach(cr =>
                {
                    var court = _mapper.Map<Court>(cr);
                    court.CourtCluster = courtCluster;
                    cr.CourtPrice.ForEach(cp =>
                        {
                            var courtPrice = _mapper.Map<CourtPrice>(cp);
                            courtPrice.Court = court;
                            court.CourtPrices.Add(courtPrice);
                        }
                    );
                    courtCluster.Courts.Add(court);
                });
                try
                {
                    await _context.AddAsync(courtCluster, cancellationToken);
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result) return Result<CourtCluster>.Failure("Fail to create court cluster.");
                    return Result<CourtCluster>.Success(_context.Entry(courtCluster).Entity);

                }
                catch (DbUpdateException ex)
                {
                    // Ghi log thông tin lỗi để phân tích
                    var errorMessage = ex.InnerException?.Message ?? ex.Message;
                    return Result<CourtCluster>.Failure($"Database update error: {errorMessage}");
                }
                catch (Exception ex)
                {
                    // Ghi log thông tin lỗi khác
                    return Result<CourtCluster>.Failure($"An error occurred: {ex.Message}");
                }

            }
        }
    }
}