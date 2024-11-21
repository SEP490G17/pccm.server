using Application.DTOs;
using Application.Handler.Users;
using Application.SpecParams;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> GetUsers([FromQuery] BaseSpecParam baseSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecParam = baseSpecParam }, ct));
        }

        [AllowAnonymous]
        [HttpGet("{username}")]

        public async Task<IActionResult> GetUser(string username, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { username = username }, ct));
        }

        [AllowAnonymous]
        [HttpPut("changestatus/{username}/{status}")]
        public async Task<IActionResult> ChangeStatus(string username, bool status)
        {
            return HandleResult(await Mediator.Send(new ActiveUser.Command() { user = new ActiveDto(){username  = username, IsActive = status} }));
        }

        // [HttpPost]
        // [AllowAnonymous]
        // public async Task<IActionResult> IsActive(ActiveDto activeDTO, CancellationToken ct)
        // {
        //     return HandleResult(await Mediator.Send(new ActiveUser.Command() { user = activeDTO }, ct));
        // }

        [AllowAnonymous]
        [HttpGet("Profile")]
        public async Task<ActionResult<AppUser>> ViewProfile()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(p => p.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return user;
        }

        [AllowAnonymous]
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile(ProfileInputDto profileInputDto, CancellationToken ct)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(p => p.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return HandleResult(await Mediator.Send(new EditProfile.Command() { User = user, ProfileInputDto = profileInputDto }, ct));
        }
    }
}
