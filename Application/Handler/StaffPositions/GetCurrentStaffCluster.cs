using Application.Interfaces;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.StaffPositions
{
    public class GetCurrentStaffCluster
    {
        public class Query : IRequest<List<int>> { }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, UserManager<AppUser> _userManager) : IRequestHandler<Query, List<int>>
        {

            public async Task<List<int>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<int> courtClusterId = null;
                var user = await _userManager.FindByNameAsync(_userAccessor.GetUserName());
                var staffDetails = await _unitOfWork.Repository<StaffDetail>().QueryList(null)
                .Include(x => x.StaffAssignments)
                .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
                if (staffDetails != null)
                {
                    courtClusterId = staffDetails.StaffAssignments.Select(x => x.CourtClusterId).ToList();
                    if (courtClusterId.Count == 0)
                    {
                        courtClusterId.Add(-1);
                    }
                }
                return courtClusterId;
            }
        }
    }
}