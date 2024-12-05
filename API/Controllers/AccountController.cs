using API.DTOs;
using API.Services;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
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
        private readonly string urlPCCM = "http://localhost:3000/";
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService, IEmailService emailService, ISendSmsService sendSmsService, IMapper mapper, DataContext context)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _sendSmsService = sendSmsService;
            _mapper = mapper;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(loginDto.Username) || x.UserName.Equals(loginDto.Username));
            if (user is null) return Unauthorized("Tên đăng nhập/ Mật khẩu không đúng");
            if (!user.LockoutEnabled) return Unauthorized("Tài khoản đã bị vô hiệu hoá");
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result)
            {
                return await CreateUserObject(user);
            }
            return Unauthorized("Tên đăng nhập/ Mật khẩu không đúng");
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

        [AllowAnonymous]
        [HttpPost("registerByStaff")]
        public async Task<ActionResult<UserDto>> RegisterByStaff(RegisterDto registerDto)
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
                var res = _mapper.Map<UserDto>(await _userManager.FindByEmailAsync(registerDto.Email));
                return res;
            }
            return BadRequest(result.Errors);
        }

        [AllowAnonymous]
        [HttpPost("createStaff")]
        public async Task<ActionResult<StaffDto>> CreateStaff(StaffInputDto staffInput)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == staffInput.userName))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                return ValidationProblem();
            }
            if (await _userManager.Users.AnyAsync(x => x.Email == staffInput.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return ValidationProblem();
            }
            if (await _userManager.Users.AnyAsync(x => x.PhoneNumber == staffInput.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại");
                return ValidationProblem();
            }
            var user = new AppUser
            {
                Email = staffInput.Email,
                UserName = staffInput.userName,
                FirstName = staffInput.FirstName,
                LastName = staffInput.LastName,
                PhoneNumber = staffInput.PhoneNumber,
                JoiningDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, staffInput.Password);

            if (result.Succeeded)
            {
                var staff = await _userManager.FindByEmailAsync(staffInput.Email);
                var staffdetails = new StaffDetail()
                {
                    UserId = staff.Id,
                    PositionId = staffInput.PositionId,
                    Salary = 0
                };
                await _context.StaffDetails.AddAsync(staffdetails);
                var _result = await _context.SaveChangesAsync();
                if (_result <= 0) return BadRequest("Error saving staff details.");

                var position = await _context.StaffPositions.FirstOrDefaultAsync(s => s.Id == staffInput.PositionId);
                foreach (var role in position.DefaultRoles)
                {
                    var _role = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == role.ToLower());
                    var userRole = new IdentityUserRole<string>()
                    {
                        RoleId = _role.Id,
                        UserId = staff.Id
                    };
                    await _context.UserRoles.AddAsync(userRole);
                }

                var staffdetail = await _context.StaffDetails.FirstOrDefaultAsync(s => s.UserId == staff.Id);
                foreach (var courtCluster in staffInput.CourtCluster)
                {
                    var staffAssignment = new StaffAssignment()
                    {
                        StaffId = staffdetail.Id,
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
                .FirstOrDefaultAsync(x => x.Id == staffdetail.Id);
                var res = _mapper.Map<StaffDto>(staffdto);
                return res;
            }
            return BadRequest(result.Errors);
        }

        private async Task<UserResponseDto> CreateUserObject(AppUser user)
        {
            return new UserResponseDto
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                Image = user?.ImageUrl?.ToString(),
                Token = await _tokenService.CreateToken(user),
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

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ProfileDto>> GetProfileUser()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(p => p.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
            var res = _mapper.Map<ProfileDto>(user);
            return res;
        }

        [HttpPost("updateProfile")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> UpdateProfileUser([FromBody] ProfileUpdateDto request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(p => p.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
            if (DateTime.TryParse(request.BirthDate, out DateTime birthDate))
            {
                user.BirthDate = birthDate;
            }
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Gender = request.Gender;
            user.ImageUrl = request.ImageUrl;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to update user profile.");
            }
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
                return Unauthorized("Không tìm thấy người dùng");
            }

            var token = _tokenService.CreatePasswordResetToken(user);

            var emailMessage = $@"
                <div style='font-family: Arial, sans-serif; background-color: #f4f6f9; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 600px; margin: 20px auto; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
                    <h2 style='text-align: center; color: #0056b3; margin-bottom: 20px;'>🔒 Yêu cầu đặt lại mật khẩu</h2>
                    <p style='font-size: 16px; line-height: 1.6;'>Chào bạn,</p>
                    <p style='font-size: 16px; line-height: 1.6;'>Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng nhấn vào nút dưới đây để đặt lại mật khẩu của bạn:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{urlPCCM}confirm-forgot-password?token={token}' 
                        style='display: inline-block; padding: 15px 30px; color: #fff; background-color: #0056b3; text-decoration: none; border-radius: 8px; font-size: 16px; font-weight: bold; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); transition: all 0.3s ease;'>
                            Đặt lại mật khẩu
                        </a>
                    </p>
                    <p style='font-size: 16px; line-height: 1.6; color: #666;'>Nếu bạn không yêu cầu thay đổi mật khẩu này, bạn có thể bỏ qua email này một cách an toàn.</p>
                    <p style='font-size: 16px; line-height: 1.6;'>Trân trọng,<br><b style='color: #0056b3;'>Hệ thống PCCM</b></p>
                    <footer style='text-align: center; margin-top: 20px; font-size: 12px; color: #999;'>
                        <p>© 2024 PCCM. All Rights Reserved.</p>
                    </footer>
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

            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, command.NewPassword);
            user.PasswordHash = hashedPassword;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Đặt lại mật khẩu thất bại");
            }

            var userName = string.IsNullOrEmpty(user.UserName) ? "bạn" : user.UserName;

            var emailMessage = $@"
                <div style='font-family: Arial, sans-serif; background-color: #f4f6f9; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 600px; margin: 20px auto; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
                    <h2 style='text-align: center; color: #0056b3; margin-bottom: 20px;'>🔒 Thay đổi mật khẩu tài khoản</h2>
                    <p style='font-size: 16px; line-height: 1.6;'>Xin chào <b>{userName}</b>,</p>
                    <p style='font-size: 16px; line-height: 1.6;'>Mật khẩu của bạn đã được thiết lập lại thành công. Vì lý do bảo mật, vui lòng đăng nhập và thay đổi mật khẩu ngay sau khi nhận được email này.</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{urlPCCM}login' 
                        style='display: inline-block; padding: 15px 30px; color: #fff; background-color: #0056b3; text-decoration: none; border-radius: 8px; font-size: 16px; font-weight: bold; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); transition: all 0.3s ease;'>
                            Nhấn vào đây để đăng nhập
                        </a>
                    </p>
                    <p style='font-size: 16px; line-height: 1.6; color: #666;'>Nếu bạn không yêu cầu thay đổi mật khẩu, vui lòng bỏ qua email này hoặc liên hệ với chúng tôi ngay lập tức để được hỗ trợ.</p>
                    <p style='font-size: 16px; line-height: 1.6;'>Trân trọng,<br><b style='color: #0056b3;'>Hệ thống PCCM</b></p>
                    <footer style='text-align: center; margin-top: 20px; font-size: 12px; color: #999;'>
                        <p>© 2024 PCCM. All Rights Reserved.</p>
                    </footer>
                </div>
                ";
            try
            {
                // Gửi email
                await _emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu", emailMessage);
                return Ok("Mật khẩu đã được thay đổi thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gửi email thất bại: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ConfirmChangePasswordDto request)
        {
            // // Validate token
            var user = await _userManager.Users.FirstOrDefaultAsync(p => p.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng");
            }

            // Kiểm tra mật khẩu hiện tại
            if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
            {
                return BadRequest("Mật khẩu hiện tại không chính xác.");
            }

            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, request.NewPassword);
            user.PasswordHash = hashedPassword;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                var emailMessage = $@"
                    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 5px; max-width: 600px; margin: 20px auto;'>
                        <h2 style='text-align: center; color: #007BFF;'>Thay đổi mật khẩu</h2>
                        <p>Chào bạn,</p>
                        <p>Tài khoản của bạn vừa thay đổi mật khẩu mật khẩu thành công.</p>
                        <p>Nếu bạn không thực hiện yêu cầu thay đổi mật khẩu này, bạn có thể thực hiện các biện pháp bảo mật để đảm bảo tài khoản của bạn.</p>
                        <p>Trân trọng,<br>Hệ thống PCCM.</p>
                    </div>
                ";

                await _emailService.SendEmailAsync(user.Email, "Change Password", emailMessage);

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
                    <table style='width: 100%; background-color: #f4f6f9; padding: 20px 0; font-family: Arial, sans-serif;'>
                        <tr>
                            <td align='center'>
                                <table style='width: 600px; background-color: #ffffff; border: 1px solid #ddd; border-radius: 10px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
                                    <tr>
                                        <td style='padding: 20px;'>
                                            <h2 style='text-align: center; color: #0056b3; margin-bottom: 20px;'>🔒 Mật khẩu mới của bạn</h2>
                                            <p style='font-size: 16px; line-height: 1.6;'>Xin chào <b>{userName}</b>,</p>
                                            <p style='font-size: 16px; line-height: 1.6;'>Mật khẩu của bạn đã được thiết lập lại thành công. Dưới đây là mật khẩu mới của bạn:</p>
                                            <p style='text-align: center; font-size: 20px; font-weight: bold; color: #333; background-color: #e9ecef; padding: 10px; border-radius: 5px; display: inline-block; margin: 20px 0;'>{newPassword}</p>
                                            <p style='font-size: 16px; line-height: 1.6;'>Vì lý do bảo mật, vui lòng đăng nhập và thay đổi mật khẩu ngay sau khi nhận được email này.</p>
                                            <div style='text-align: center; margin: 30px 0;'>
                                                <a href='{urlPCCM}login' 
                                                style='display: inline-block; padding: 15px 30px; font-size: 16px; color: #ffffff; background-color: #0056b3; text-decoration: none; border-radius: 8px; font-weight: bold; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); transition: all 0.3s ease;'>
                                                    Nhấn vào đây để đăng nhập
                                                </a>
                                            </div>
                                            <p style='font-size: 16px; line-height: 1.6; color: #666;'>Nếu bạn không yêu cầu đặt lại mật khẩu này, vui lòng bỏ qua email hoặc liên hệ với chúng tôi để được hỗ trợ.</p>
                                            <p style='font-size: 16px; line-height: 1.6;'>Trân trọng,<br><b style='color: #0056b3;'>Hệ thống PCCM</b></p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align: center; padding: 10px; font-size: 12px; color: #999;'>
                                            <p>© 2024 PCCM. All Rights Reserved.</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
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
