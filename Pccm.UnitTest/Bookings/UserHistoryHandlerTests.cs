using Application.Handler.Bookings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;
using Application.SpecParams.BookingSpecification;
using Moq;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class UserHistoryHandlerTests
    {
        private readonly IMediator _mediator;
        private readonly Mock<IUserAccessor> _userAccessorMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;

        public UserHistoryHandlerTests()
        {
            _userAccessorMock = new Mock<IUserAccessor>();
            _userManagerMock = CreateMockUserManager();

            // Thiết lập mock IHttpContextAccessor
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(
                    new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "adminstrator") }
                )
            );
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Thiết lập môi trường kiểm thử
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: true);
            });
            builder.ConfigureServices((context, services) =>
            {
                // Đăng ký IHttpContextAccessor và IUserAccessor mock
                services.AddSingleton(httpContextAccessorMock.Object);
                _userAccessorMock.Setup(x => x.GetUserName()).Returns("adminstrator");
                services.AddSingleton(_userAccessorMock.Object);

                // Đăng ký UserManager<AppUser> mock
                services.AddSingleton(_userManagerMock.Object);

                // Thêm các dịch vụ ứng dụng vào DI container
                services.AddApplicationService(context.Configuration);
            });

            var host = builder.Build();
            _mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("b6341ccf-1a22-426c-83bd-21f3f63cd83f", 0, 5, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldListUserHistory_WhenValidData(string? UserId, int skip, int pageSize)
        {
            if (this._mediator is null) return null;
            var response = await this._mediator.Send(new Application.Handler.Bookings.UserHistory.Query()
            {
                BookingSpecParam = new BookingUserHistorySpecParam()
                {
                    UserId = UserId,
                    BookingStatus = Domain.Enum.BookingStatus.Pending,
                    PaymentStatus = Domain.Enum.PaymentStatus.Pending,
                }
            });

            return response.IsSuccess;
        }

        private static BookingWithComboDto CreateBookingDto(
            int courtId,
            int comboId,
            string fullName,
            string phoneNumber,
            string fromDate,
            string fromTime,
            string toTime)
        {
            return new BookingWithComboDto
            {
                CourtId = courtId,
                ComboId = comboId,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                FromDate = DateTime.Parse(fromDate),
                FromTime = TimeOnly.Parse(fromTime),
                ToTime = TimeOnly.Parse(toTime)
            };
        }

        private Mock<UserManager<AppUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(
                store.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            // Mock FindByNameAsync để trả về AppUser khi được gọi
            userManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string userName) =>
                {
                    if (userName == "adminstrator")
                    {
                        return new AppUser
                        {
                            Id = "d6341ccf-1a22-426c-83bd-21f3f63cd83f",
                            UserName = "adminstrator",
                            FirstName = "Alexandros",
                            LastName = "Papadopoulos",
                            Email = "adminstrator@test.com"
                        };
                    }
                    return null;
                });

            // Mock GetRolesAsync để trả về danh sách roles
            userManagerMock
                .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(new List<string> { "Owner" });

            return userManagerMock;
        }

    }
}
