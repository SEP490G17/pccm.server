using Application.Core;
using Application.DTOs;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.StaffPositions
{
    public class ApplyToAll
    {
        public class Command : IRequest<Result<List<StaffPosition>>>
        {
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
                var staffDetails = await _context.StaffDetails.Include(sd => sd.Position).ToListAsync();

                foreach (var staffDetail in staffDetails)
                {
                    var userRoles = await _context.UserRoles
                              .Where(ur => ur.UserId == staffDetail.UserId)
                              .ToListAsync();

                    _context.UserRoles.RemoveRange(userRoles);

                    var roles = await _context.Roles.ToListAsync();
                    var roleDict = roles.ToDictionary(r => r.NormalizedName, r => r.Id);
                    foreach (var role in staffDetail.Position.DefaultRoles)
                    {
                        if (roleDict.TryGetValue(role.ToUpper(), out var roleId))
                        {
                            _context.UserRoles.Add(new IdentityUserRole<string>
                            {
                                UserId = staffDetail.UserId,
                                RoleId = roleId
                            });
                        }
                    }
                }
                await _context.SaveChangesAsync();
                List<StaffPosition> res = await _context.StaffPositions.ToListAsync();
                return Result<List<StaffPosition>>.Success(res);
            }
        }
    }
}