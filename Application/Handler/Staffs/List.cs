using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.CourtCountSpecification;
using Application.SpecParams.StaffSpecification;
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
        public class Query : IRequest<Result<Pagination<StaffDto>>>
        {
            public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork, UserManager<AppUser> _userManager) : IRequestHandler<Query, Result<Pagination<StaffDto>>>
        {
            public async Task<Result<Pagination<StaffDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecWithFilterParam;

                var spec = new StaffSpecification(querySpec);
                var specCount = new StaffCountSpecification(querySpec);

                var totalElement = await _unitOfWork.Repository<StaffDetail>().CountAsync(specCount, cancellationToken);

                 var data = await _unitOfWork.Repository<StaffDetail>().QueryList(spec)
                 .Include(x => x.User).Include(x => x.Position).Include(x => x.StaffAssignments).ThenInclude(x => x.CourtCluster).ToListAsync(cancellationToken);
                var staffs = _mapper.Map<List<StaffDto>>(data);
                for (int i = 0; i < staffs.Count; i++)
                {
                    var roles = await _userManager.GetRolesAsync(data[i].User);
                    staffs[i].Roles = roles.ToArray();
                }

                return Result<Pagination<StaffDto>>.Success(new Pagination<StaffDto>(querySpec.PageSize, totalElement, staffs));
            }
        }

    }
}