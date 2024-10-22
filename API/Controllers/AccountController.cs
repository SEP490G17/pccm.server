using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Claims;
using API.Services;
using Application.DTOs;
using Application.Handler.Users;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(loginDto.Email));
            if (user is null) return Unauthorized();
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result)
            {
                return CreateUserObject(user);
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            {
                ModelState.AddModelError("Username", "Email taken");
                return ValidationProblem();
            }
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("Email", "Email taken");
                return ValidationProblem();
            }
            var user = new AppUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Username
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }
            return BadRequest(result.Errors);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                // Image = user?.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
                Token = _tokenService.CreateToken(user),
                UserName = user.UserName
            };
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(p => p.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
            return CreateUserObject(user);
        }

        [AllowAnonymous]
        [HttpPost("Profile")]
        public async Task<ActionResult<AppUser>> viewProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return user;
        }

        [AllowAnonymous]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole(roleName);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return Ok("Role created successfully.");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest("Role already exists.");
        }

        [AllowAnonymous]
        [HttpPost("AddUserRole")]
        public async Task<IActionResult> AddUserToRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    return Ok("User added to role successfully.");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return NotFound("User not found.");
        }

    }
}
