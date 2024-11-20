using API.DTOs;
using API.Services;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


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
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService, IEmailService emailService, ISendSmsService sendSmsService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _sendSmsService = sendSmsService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(loginDto.Username) || x.UserName.Equals(loginDto.Username));
            if (user is null) return BadRequest("Tên đăng nhập/ Mật khẩu không đúng");
            if (user.IsDisabled) return StatusCode(403, "Tài khoản đã bị vô hiệu hóa");
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result)
            {
                return await CreateUserObject(user);
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                return ValidationProblem();
            }
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return ValidationProblem();
            }
            if (await _userManager.Users.AnyAsync(x => x.PhoneNumber == registerDto.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại");
                return ValidationProblem();
            }
            var user = new AppUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Username,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                JoiningDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        private async Task<UserResponseDto> CreateUserObject(AppUser user)
        {
            return new UserResponseDto
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                Image = user?.ImageUrl?.ToString(),
                Token = _tokenService.CreateToken(user),
                UserName = user.UserName,
                PhoneNumber = user?.PhoneNumber,
                Roles = await _userManager.GetRolesAsync(user),
            };
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(p => p.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
            return await CreateUserObject(user);
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
                return NotFound("Không tìm thấy người dùng");
            }

            var token = _tokenService.CreatePasswordResetToken(user);

            var emailMessage = $@"
                <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 5px; max-width: 600px; margin: 20px auto;'>
                    <h2 style='text-align: center; color: #007BFF;'>Yêu cầu đặt lại mật khẩu</h2>
                    <p>Chào bạn,</p>
                    <p>Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng nhấn vào liên kết dưới đây để đặt lại mật khẩu của bạn:</p>
                    <p style='text-align: center;'>
                        <a href='https://argonaut.asia/confirm-forgot-password?token={token}' style='display: inline-block; padding: 10px 20px; color: #fff; background-color: #007BFF; text-decoration: none; border-radius: 5px;'>Đặt lại mật khẩu</a>
                    </p>
                    <p>Nếu bạn không yêu cầu thay đổi mật khẩu này, bạn có thể bỏ qua email này một cách an toàn.</p>
                    <p>Trân trọng,<br>Hệ thống PCCM.</p>
                </div>
            ";

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Reset Password", emailMessage);
                return Ok("Yêu cầu đặt lại mật khẩu đã được gửi.");
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
                return BadRequest("Token không hợp lệ.");
            }

            // Extract user email from token
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng");
            }

            // Tạo mật khẩu mới
            var newPassword = GenerateSecurePassword();

            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, newPassword);
            user.PasswordHash = hashedPassword;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Đặt lại mật khẩu thất bại");
            }

            var userName = string.IsNullOrEmpty(user.UserName) ? "bạn" : user.UserName;

            var emailMessage = $@"
                <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 5px; max-width: 600px; margin: 20px auto;'>
                    <h2 style='text-align: center; color: #007BFF;'>Mật khẩu mới của bạn</h2>
                    <p>Xin chào {userName},</p>
                    <p>Mật khẩu của bạn đã được thiết lập lại thành công. Dưới đây là mật khẩu mới của bạn:</p>
                    <p style='text-align: center; font-size: 20px; font-weight: bold; color: #333;'>{newPassword}</p>
                    <p>Vì lý do bảo mật, hãy đăng nhập và thay đổi mật khẩu ngay sau khi nhận được email này.</p>
                    <p style='text-align: center;'>
                        <a href='https://argonaut.asia/login' style='display: inline-block; padding: 10px 20px; color: #fff; background-color: #007BFF; text-decoration: none; border-radius: 5px;'>Nhấn vào đây để đăng nhập</a>
                    </p>
                    <p>Trân trọng,<br>Hệ thống PCCM.</p>
                </div>
            ";

            try
            {
                // Gửi email
                await _emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu", emailMessage);
                return Ok("Mật khẩu mới đã được gửi đến email của bạn.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gửi email thất bại: {ex.Message}");
            }
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
                return NotFound("Không tìm thấy người dùng");
            }

            // Kiểm tra mật khẩu hiện tại
            if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
            {
                return BadRequest("Mật khẩu hiện tại không chính xác.");
            }

            // Tạo token để thay đổi mật khẩu
            var token = _tokenService.CreatePasswordResetToken(user);

            var emailMessage = $@"
                <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 5px; max-width: 600px; margin: 20px auto;'>
                    <h2 style='text-align: center; color: #007BFF;'>Yêu cầu thay đổi mật khẩu</h2>
                    <p>Chào bạn,</p>
                    <p>Bạn đã yêu cầu thay đổi mật khẩu. Vui lòng nhấn vào liên kết dưới đây để thay đổi mật khẩu của bạn:</p>
                    <p style='text-align: center;'>
                        <a href='http://localhost:5000/api/Account/confirm-change-password?token={token}' style='display: inline-block; padding: 10px 20px; color: #fff; background-color: #007BFF; text-decoration: none; border-radius: 5px;'>Thay đổi mật khẩu</a>
                    </p>
                    <p>Nếu bạn không yêu cầu thay đổi mật khẩu này, bạn có thể bỏ qua email này một cách an toàn.</p>
                    <p>Trân trọng,<br>Hệ thống PCCM.</p>
                </div>
            ";

            await _emailService.SendEmailAsync(user.Email, "Change Password", emailMessage);

            return Ok("Một email đã được gửi đến bạn để thay đổi mật khẩu.");
        }

        [AllowAnonymous]
        [HttpPost("confirm-change-password")]
        public async Task<IActionResult> ConfirmChangePassword([FromBody] ConfirmChangePasswordDto request)
        {
            // Validate token
            var principal = _tokenService.ValidateToken(request.Token);
            if (principal == null)
            {
                return BadRequest("Token không hợp lệ");
            }

            // Extract user email from token
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng");
            }

            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, request.NewPassword);
            user.PasswordHash = hashedPassword;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("Mật khẩu đã được thay đổi thành công.");
            }
            return BadRequest(result.Errors);
        }

        public class SendMessOtp
        {
            public string To { get; set; }
            public string Text { get; set; }
        }
        [HttpPost("test-sendsms")]
        [AllowAnonymous]
        public async Task<IActionResult> SendSmsTest([FromBody] SendMessOtp sendMessOtp, CancellationToken cancellationToken)
        {
            await _sendSmsService.SendSms(sendMessOtp.To, sendMessOtp.Text, cancellationToken);
            return Ok("Send sms success");
        }


        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordDTO request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email));
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng");
            }

            // Tạo mật khẩu mới
            var newPassword = GenerateSecurePassword();

            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, newPassword);
            user.PasswordHash = hashedPassword;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Đặt lại mật khẩu thất bại");
            }

            var userName = string.IsNullOrEmpty(user.UserName) ? "bạn" : user.UserName;

            var emailMessage = $@"
                <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 5px; max-width: 600px; margin: 20px auto;'>
                    <h2 style='text-align: center; color: #007BFF;'>Mật khẩu mới của bạn</h2>
                    <p>Xin chào {userName},</p>
                    <p>Mật khẩu của bạn đã được thiết lập lại thành công. Dưới đây là mật khẩu mới của bạn:</p>
                    <p style='text-align: center; font-size: 20px; font-weight: bold; color: #333;'>{newPassword}</p>
                    <p>Vì lý do bảo mật, hãy đăng nhập và thay đổi mật khẩu ngay sau khi nhận được email này.</p>
                    <p style='text-align: center;'>
                        <a href='https://argonaut.asia/login' style='display: inline-block; padding: 10px 20px; color: #fff; background-color: #007BFF; text-decoration: none; border-radius: 5px;'>Nhấn vào đây để đăng nhập</a>
                    </p>
                    <p>Trân trọng,<br>Hệ thống PCCM.</p>
                </div>
            ";

            try
            {
                // Gửi email
                await _emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu", emailMessage);
                return Ok("Mật khẩu mới đã được gửi đến email của bạn.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gửi email thất bại: {ex.Message}");
            }
        }

        // Hàm tạo mật khẩu phức tạp
        private static string GenerateSecurePassword()
        {
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+<>?";
            const int passwordLength = 12;

            var random = new Random();
            var passwordChars = new List<char>();

            // Đảm bảo mỗi loại ký tự đều có ít nhất 1 ký tự
            passwordChars.Add(lowerCase[random.Next(lowerCase.Length)]);
            passwordChars.Add(upperCase[random.Next(upperCase.Length)]);
            passwordChars.Add(digits[random.Next(digits.Length)]);
            passwordChars.Add(specialChars[random.Next(specialChars.Length)]);

            // Thêm các ký tự ngẫu nhiên còn lại
            var allChars = lowerCase + upperCase + digits + specialChars;
            while (passwordChars.Count < passwordLength)
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }

            // Trộn thứ tự ký tự
            return new string(passwordChars.OrderBy(x => random.Next()).ToArray());
        }


    }
}
