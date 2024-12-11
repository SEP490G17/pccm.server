using Application.Core;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Staffs
{
    public class Detail
    {
        public class Query : IRequest<Result<StaffDto>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<StaffDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }
            public async Task<Result<StaffDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var staff = await _context.StaffDetails
                .Include(a => a.User)
                .Include(a => a.Position)
                .Include(a => a.StaffAssignments)
                .ThenInclude(a => a.CourtCluster)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (staff == null) return Result<StaffDto>.Failure("Staff not found!");
                var roles = await _context.UserRoles
                .Where(ur => ur.UserId == staff.UserId)
                .Join(
                    _context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r
                )
                .ToListAsync(cancellationToken);

                var rolesAdd = new List<IdentityRole>();
                if (roles.Count() > staff.Position.DefaultRoles.Length)
                {
                    foreach (var item in roles)
                    {
                        bool exist = staff.Position.DefaultRoles.Contains(item.Name);
                        if (!exist) rolesAdd.Add(item);
                    }
                }
                var staffDtoMap = _mapper.Map<StaffDto>(staff);
                staffDtoMap.Roles = staffDtoMap.Roles.Concat(rolesAdd.Select(role => role.Name)).ToArray();
                return Result<StaffDto>.Success(staffDtoMap);
            }
        }
    }
}
