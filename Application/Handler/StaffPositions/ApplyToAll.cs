using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
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
                List<StaffRoleInputDto> _roles = request.data;
                var staffDetails = await _context.StaffDetails
                .Include(s => s.Position)
                .ToListAsync();
                var roles = await _context.Roles.ToListAsync();
                var roleDict = roles.ToDictionary(r => r.NormalizedName.ToUpper(), r => r.Id);
                var userRolesToAdd = new List<IdentityUserRole<string>>();
                var newPositon = new List<StaffPosition>();

                foreach (var staffDetail in staffDetails)
                {
                    //lấy ra các role cũ
                    var defaultRoleIds = staffDetail.Position.DefaultRoles
                        .Where(role => roleDict.ContainsKey(role.ToUpper()))
                        .Select(role => roleDict[role.ToUpper()])
                        .ToList();
                    //xóa các role cũ trong userrole đi
                    var rolesToRemove = await _context.UserRoles
                        .Where(ur => ur.UserId == staffDetail.UserId && defaultRoleIds.Contains(ur.RoleId))
                        .ToListAsync();
                    _context.UserRoles.RemoveRange(rolesToRemove);
                    var result = await _context.SaveChangesAsync() > 0;
                    if (!result) return Result<List<StaffPosition>>.Failure("Fail to update roles");
                }

                foreach (var item in _roles)
                {
                    var staffPosition = await _context.StaffPositions.FirstOrDefaultAsync(sp => sp.Name == item.name);
                    if (staffPosition != null)
                    {
                        staffPosition.DefaultRoles = item.defaultRoles;
                        newPositon.Add(staffPosition);
                    }
                }

                foreach (var staffDetail in staffDetails)
                {
                    var defaultNewRoleIds = new List<string>();
                    foreach (var item in newPositon)
                    {
                        if (staffDetail.Position.Id == item.Id && staffDetail.Position.Name.ToUpper() == item.Name.ToUpper())
                        {
                            staffDetail.Position.DefaultRoles = item.DefaultRoles;
                        }
                        defaultNewRoleIds = staffDetail.Position.DefaultRoles
                        .Where(role => roleDict.ContainsKey(role.ToUpper()))
                        .Select(role => roleDict[role.ToUpper()])
                        .ToList();
                    }
                    foreach (var roleId in defaultNewRoleIds)
                    {
                        var roleExist = await _context.UserRoles.FirstOrDefaultAsync(u => u.RoleId == roleId && u.UserId == staffDetail.UserId);
                        if (roleExist == null)
                        {
                            userRolesToAdd.Add(new IdentityUserRole<string>
                            {
                                UserId = staffDetail.UserId,
                                RoleId = roleId
                            });
                        }
                    }
                }

                _context.StaffPositions.UpdateRange(newPositon);
                await _context.UserRoles.AddRangeAsync(userRolesToAdd);

                await _context.SaveChangesAsync();
                List<StaffPosition> res = await _context.StaffPositions.ToListAsync();
                return Result<List<StaffPosition>>.Success(res);
            }
        }
    }
}