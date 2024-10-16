using System.Security.Claims;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<AppUser> _userManager, TokenService _tokenService) : ControllerBase
    {

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName.Equals(loginDto.Username));
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

        [AllowAnonymous]
        [HttpGet("check-email-exist")]
        public async Task<IActionResult> CheckEmailExist([FromQuery]string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email) ? Ok(true) : Ok(false);
        }

        public struct ChangePasswordDto
        {
            public string CurrentPassword { get; set; }
            public string ChangePassword { get; set; }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(p => p.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.ChangePassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully.");
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
    }
}
