using Application.Core;
using Application.DTOs;
using AutoMapper;
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
            public int CourtId { get; set; }
            public List<CourtComboDto> CourtComboCreateDtos { get; set; }


        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x).SetValidator(new CreateCourtValidator());
            }
        }

        public class Handler(DataContext _context, IMapper mapper) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var court = await _context.Courts.FirstOrDefaultAsync(x => x.Id.Equals(request.CourtId) && x.DeleteAt == null, cancellationToken);
                if (court == null)
                {
                    return Result<Unit>.Failure("Sân không tồn tại");
                }
                if (request.CourtComboCreateDtos.Count == 0)
                {
                    return Result<Unit>.Failure("Danh sách combo không được rỗng");
                }
                court.CourtCombos.Clear();
                foreach (var combo in request.CourtComboCreateDtos)
                {
                    combo.Id = 0;
                }
                court.CourtCombos = mapper.Map<List<CourtCombo>>(request.CourtComboCreateDtos);
                var courtCombos = await _context.CourtCombos.Where(c => c.CourtId == request.CourtId).ToListAsync(cancellationToken);
                if (courtCombos.Count > 0)
                {
                    _context.RemoveRange(courtCombos);
                }
                _context.Update(court);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}