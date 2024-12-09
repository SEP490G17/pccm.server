using Application.Handler.Staffs;
using Application.SpecParams;
using AutoMapper;
using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class StaffController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public StaffController(UserManager<AppUser> userManager, DataContext context, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Owner,ManagerStaff")]
        public async Task<IActionResult> GetStaffs([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerStaff")]

        public async Task<IActionResult> GetStaff(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }
             [HttpGet("edit/{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerStaff")]

        public async Task<IActionResult> GetStaffEdit(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new DetailEdit.Query() { Id = id }, ct));
        }

        [HttpPut("editStaff")]
        [Authorize(Roles = "Admin,Owner,ManagerStaff")]

        public async Task<ActionResult<StaffDto>> EditStaff(StaffInputDto data, CancellationToken ct)
        {
            var staffInput = data;
            var user = await _context.Users.Include(u => u.StaffDetail).FirstOrDefaultAsync(u => u.StaffDetail.Id == staffInput.StaffDetailId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (await _userManager.Users.AnyAsync(x => x.UserName == staffInput.userName && x.Id != user.Id))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                return ValidationProblem();
            }

            if (await _userManager.Users.AnyAsync(x => x.Email == staffInput.Email && x.Id != user.Id))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return ValidationProblem();
            }

            if (await _userManager.Users.AnyAsync(x => x.PhoneNumber == staffInput.PhoneNumber && x.Id != user.Id))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại");
                return ValidationProblem();
            }
            user.Email = staffInput.Email;
            user.UserName = staffInput.userName;
            user.FirstName = staffInput.FirstName;
            user.LastName = staffInput.LastName;
            user.PhoneNumber = staffInput.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                //update staffdetail
                var staffDetail = await _context.StaffDetails.FirstOrDefaultAsync(s => s.Id == staffInput.StaffDetailId);
                if (staffDetail == null)
                {
                    throw new Exception("staffDetail not found");
                }
                //nếu position thay đổi
                if (staffDetail.PositionId != staffInput.PositionId)
                {
                    //update position staff detail
                    staffDetail.PositionId = staffInput.PositionId;
                    _context.StaffDetails.Update(staffDetail);
                    //update role trong userrole
                    //xóa các role cũ đi
                    var OldRole = await _context.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
                    _context.UserRoles.RemoveRange(OldRole);
                    //thêm role mới lại
                    var position = await _context.StaffPositions.FirstOrDefaultAsync(s => s.Id == staffInput.PositionId);
                    foreach (var role in position.DefaultRoles)
                    {
                        var _role = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == role.ToLower());
                        var userRole = new IdentityUserRole<string>()
                        {
                            RoleId = _role.Id,
                            UserId = user.Id
                        };
                        await _context.UserRoles.AddAsync(userRole);
                    }
                }
                var OldAssignment = await _context.StaffAssignments.Where(x => x.StaffId == staffDetail.Id).ToListAsync();
                _context.StaffAssignments.RemoveRange(OldAssignment);
                await _context.SaveChangesAsync();
                foreach (var courtCluster in staffInput.CourtCluster)
                {
                    var staffAssignment = new StaffAssignment()
                    {
                        StaffId = staffDetail.Id,
                        CourtClusterId = courtCluster,
                        CourtCluster = await _context.CourtClusters.FirstOrDefaultAsync(c => c.Id == courtCluster)
                    };
                    await _context.StaffAssignments.AddAsync(staffAssignment);
                }
                var result1 = await _context.SaveChangesAsync();
                if (result1 < 0) return BadRequest("Error saving staff details.");
                var staffdto = await _context.StaffDetails
                .Include(s => s.User)
                .Include(s => s.Position)
                .Include(s => s.StaffAssignments)
                .ThenInclude(x => x.CourtCluster)
                .FirstOrDefaultAsync(x => x.Id == staffDetail.Id);
                var res = _mapper.Map<StaffDto>(staffdto);
                return res;
            }
            return BadRequest(result.Errors);
        }
    }
}