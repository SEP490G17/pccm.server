using Application.Core;
using Application.DTOs;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.StaffPositions
{
    public class UpdateRole
    {
        public class Command : IRequest<Result<List<StaffPosition>>>
        {
            public List<StaffRoleInputDto> data { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {

            }
        }
        public class Handler : IRequestHandler<Command, Result<List<StaffPosition>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<List<StaffPosition>>> Handle(Command request, CancellationToken cancellationToken)
            {
                List<StaffRoleInputDto> roles = request.data;
                foreach (var item in roles)
                {
                    StaffPosition staffPosition = await _context.StaffPositions.FirstOrDefaultAsync(sp => sp.Name == item.name);
                    if (staffPosition != null)
                    {
                        staffPosition.DefaultRoles = item.defaultRoles;
                        _context.StaffPositions.Update(staffPosition);
                    }
                }
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<List<StaffPosition>>.Failure("Fail to update roles");
                List<StaffPosition> res = await _context.StaffPositions.ToListAsync();
                return Result<List<StaffPosition>>.Success(res);
            }
        }
    }
}