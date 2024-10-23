using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Staffs
{
    public class List
    {
        public class Query : IRequest<Result<List<StaffDto>>> { }

        public class Handler(IUnitOfWork _unitOfWork, IMapper _mapper, UserManager<AppUser> _userManager) : IRequestHandler<Query, Result<List<StaffDto>>>
        {
            public async Task<Result<List<StaffDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = await _unitOfWork.Repository<StaffDetail>()
                .QueryList(null).Include(x=>x.User).Include(x=>x.Position)
                .Include(x=>x.StaffAssignments).ThenInclude(x=>x.CourtCluster)
                .ToListAsync(cancellationToken);
                var staffs = _mapper.Map<List<StaffDto>>(query);
                for (int i = 0; i < staffs.Count; i++)
                {
                    var roles = await _userManager.GetRolesAsync(query[i].User);
                    staffs[i].Roles = roles.ToArray();
                }
                return Result<List<StaffDto>>.Success(staffs);
            }
        }

    }
}