

using Application.Core;
using Application.DTOs;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.CourtCombos
{
    public class CreateCourtCombo
    {

        public class Command : IRequest<Result<Unit>>
        {

            public List<CourtComboCreateDto> CourtComboCreateDtos { get; set; }


        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x).SetValidator(new CreateCourtValidator());
            }
        }

        public class Handler(DataContext _context) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var combos = request.CourtComboCreateDtos;
                if (combos == null || !combos.Any())
                {
                    return Result<Unit>.Failure("Không có combo để sử lý.");
                }
                var courtCombos = new List<CourtCombo>();

                foreach (var combo in combos)
                {

                    var court = await _context.Courts.FirstOrDefaultAsync(x => x.Id == combo.CourtId);
                    if (court != null)
                    {
                        var courtCombo = new CourtCombo
                        {
                            DisplayName = combo.DisplayName,
                            TotalPrice = combo.TotalPrice,
                            Duration = combo.Duration,
                            Court = court,
                        };
                        courtCombos.Add(courtCombo);
                    }
                }
                await _context.CourtCombos.AddRangeAsync(courtCombos);
                await _context.SaveChangesAsync();
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}