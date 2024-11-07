using System.Security.Claims;
using API.DTOs;
using API.Services;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ISendSmsService _sendSmsService;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService,  IEmailService emailService, ISendSmsService sendSmsService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _sendSmsService = sendSmsService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody]LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(loginDto.Username) || x.UserName.Equals(loginDto.Username));
            if (user is null) return Unauthorized();
            if(user.IsDisabled) return StatusCode(403, "Tài khoản đã bị vô hiệu hóa");
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result)
            {
                return CreateUserObject(user);
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(RegisterDto registerDto)
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
                UserName = registerDto.Username,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
                
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }
            return BadRequest(result.Errors);
        }

        private UserResponseDto CreateUserObject(AppUser user)
        {
            return new UserResponseDto
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                // Image = user?.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
                Token = _tokenService.CreateToken(user),
                UserName = user.UserName
            };
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(p => p.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
            return CreateUserObject(user);
        }

        [AllowAnonymous]
        [HttpPost("Profile")]
        public async Task<ActionResult<AppUser>> ViewProfile()
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


        // API khi người dùng yêu cầu quên mật khẩu
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email));
            if (user == null)
            {
                return NotFound("User not found");
            }

            var token = _tokenService.CreatePasswordResetToken(user);

            var emailMessage = $@"
                    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 5px; max-width: 600px; margin: 20px auto;'>
                        <h2 style='text-align: center; color: #007BFF;'>Password Reset Request</h2>
                        <p>Hi,</p>
                        <p>You have requested to reset your password. Please click the link below to reset your password:</p>
                        <p style='text-align: center;'>
                            <a href='https://argonaut.asia/confirm-forgot-password?token={token}' style='display: inline-block; padding: 10px 20px; color: #fff; background-color: #007BFF; text-decoration: none; border-radius: 5px;'>Reset Password</a>
                        </p>
                        <p>If you didn't request this, you can safely ignore this email.</p>
                        <p>Thanks,<br>PCCM System.</p>
                    </div>
                ";
            try
            {
                await _emailService.SendEmailAsync(user.Email, "Reset Password", emailMessage);
                return Ok("Reset password email sent.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }

        // API xác nhận reset mật khẩu sau khi click vào link email
        [AllowAnonymous]
        [HttpPost("confirm-forgot-password")]
        public async Task<IActionResult> ConfirmForgotPassword([FromBody] ConfirmForgotPasswordDto command)
        {
            // Validate token
            var principal = _tokenService.ValidateToken(command.Token);
            if (principal == null)
            {
                return BadRequest("Invalid or expired token");
            }

            // Extract user email from token
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, command.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password has been reset successfully.");
            }
            return BadRequest(result.Errors);
        }

        // API khi người dùng yêu cầu thay đổi mật khẩu (nhập mật khẩu hiện tại)
        [AllowAnonymous]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            // Tìm người dùng theo email
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.Equals(request.Email));
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Kiểm tra mật khẩu hiện tại
            if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
            {
                return BadRequest("Current password is incorrect");
            }

            // Tạo token để thay đổi mật khẩu
            var token = _tokenService.CreatePasswordResetToken(user);

            var emailMessage = $@"
                    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 5px; max-width: 600px; margin: 20px auto;'>
                        <h2 style='text-align: center; color: #007BFF;'>Password Change Request</h2>
                        <p>Hi,</p>
                        <p>You have requested to change your password. Please click the link below to change your password:</p>
                        <p style='text-align: center;'>
                            <a href='http://localhost:5000/api/Account/confirm-change-password?token={token}' style='display: inline-block; padding: 10px 20px; color: #fff; background-color: #007BFF; text-decoration: none; border-radius: 5px;'>Change Password</a>
                        </p>
                        <p>If you didn't request this, you can safely ignore this email.</p>
                        <p>Thanks,<br>PCCM System.</p>
                    </div>
                ";
            await _emailService.SendEmailAsync(user.Email, "Change Password", emailMessage);

            return Ok("An email has been sent to change your password.");
        }

        [AllowAnonymous]
        [HttpPost("confirm-change-password")]
        public async Task<IActionResult> ConfirmChangePassword([FromBody] ConfirmChangePasswordDto request)
        {
            // Validate token
            var principal = _tokenService.ValidateToken(request.Token);
            if (principal == null)
            {
                return BadRequest("Invalid or expired token");
            }

            // Extract user email from token
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }
            //var result = await _userManager.ChangePasswordAsync(user, user.PasswordHash, request.NewPassword);
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password has been reset successfully.");
            }
            return BadRequest(result.Errors);
        }

        public class SendMessOtp{
            public string To { get; set; }
            public string Text { get; set; }
        }
        [HttpPost("test-sendsms")]
        [AllowAnonymous]
        public async Task<IActionResult> SendSmsTest([FromBody]SendMessOtp sendMessOtp, CancellationToken cancellationToken){
            await _sendSmsService.SendSms(sendMessOtp.To,sendMessOtp.Text, cancellationToken);
            return Ok("Send sms success");
        }
    }
}
