using Application.Core;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Staffs
{
    public class DetailEdit
    {
        public class Query : IRequest<Result<StaffDetailDto>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<StaffDetailDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }
            public async Task<Result<StaffDetailDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var staff = await _context.StaffDetails
                .Include(a => a.User)
                .Include(a => a.Position)
                .Include(a => a.StaffAssignments)
                .ThenInclude(a => a.CourtCluster)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (staff == null) return Result<StaffDetailDto>.Failure("Staff not found!");

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
                var res = _mapper.Map<StaffDetailDto>(staff);
                res.RoleAdd = rolesAdd;
                return Result<StaffDetailDto>.Success(res);
            }
        }
    }
}
