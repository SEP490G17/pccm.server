using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Extensions;
using Application.DTOs;
using Application.Handler.Bookings;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;

namespace Pccm.UnitTest.Bookings
{
    public class BookingWithComboHandlerTests
    {
        private readonly IMediator _mediator;
        private readonly Mock<IUserAccessor> _userAccessorMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;

        public BookingWithComboHandlerTests()
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

        [TestCase(20, 1, "John Doe", "123456789", "2025-11-02", "10:00", "12:00", ExpectedResult = true)]
        public async Task<bool> Handle_ShouldBookingWithComboTests(
            int courtId,
            int comboId,
            string fullName,
            string phoneNumber,
            string fromDate,
            string fromTime,
            string toTime)
        {
            try
            {
                var bookingWithComboDto = CreateBookingDto(courtId, comboId, fullName, phoneNumber, fromDate, fromTime, toTime);

                var result = await _mediator.Send(new BookingWithCombo.Command { Booking = bookingWithComboDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
        }

         
         [TestCase(20, 2000, "John Doe", "123456789", "2025-11-01", "10:00", "12:00", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldBookingWithComboTestsFail_WhenComboIDNotExist(
            int courtId,
            int comboId,
            string fullName,
            string phoneNumber,
            string fromDate,
            string fromTime,
            string toTime)
        {
            try
            {
                var bookingWithComboDto = CreateBookingDto(courtId, comboId, fullName, phoneNumber, fromDate, fromTime, toTime);

                var result = await _mediator.Send(new BookingWithCombo.Command { Booking = bookingWithComboDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
        }

        [TestCase(2000, 1, "John Doe", "123456789", "2025-11-01", "10:00", "12:00", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldBookingWithComboTestsFail_WhenCourtIDNotExist(
            int courtId,
            int comboId,
            string fullName,
            string phoneNumber,
            string fromDate,
            string fromTime,
            string toTime)
        {
            try
            {
                var bookingWithComboDto = CreateBookingDto(courtId, comboId, fullName, phoneNumber, fromDate, fromTime, toTime);

                var result = await _mediator.Send(new BookingWithCombo.Command { Booking = bookingWithComboDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
        }

         [TestCase(2000, 1, "John Doe", "123456789", "2024-09-01", "10:00", "12:00", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldBookingWithComboTestsFail_WhenBookingDateBeforeCurrentDate(
            int courtId,
            int comboId,
            string fullName,
            string phoneNumber,
            string fromDate,
            string fromTime,
            string toTime)
        {
            try
            {
                var bookingWithComboDto = CreateBookingDto(courtId, comboId, fullName, phoneNumber, fromDate, fromTime, toTime);

                var result = await _mediator.Send(new BookingWithCombo.Command { Booking = bookingWithComboDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
        }

        [TestCase(2000, 1, "John Doe", "123456789", "2024-11-11", "10:00", "12:00", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldBookingWithComboTestsFail_WhenDuplicateBooking(
            int courtId,
            int comboId,
            string fullName,
            string phoneNumber,
            string fromDate,
            string fromTime,
            string toTime)
        {
            try
            {
                var bookingWithComboDto = CreateBookingDto(courtId, comboId, fullName, phoneNumber, fromDate, fromTime, toTime);

                var result = await _mediator.Send(new BookingWithCombo.Command { Booking = bookingWithComboDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
        }

         [TestCase(20, 1, "John Doe", "123456789", "2025-11-01", "10:00", "12:00", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldBookingWithComboTestsFail_WhenDuplicateBookingCombo(
            int courtId,
            int comboId,
            string fullName,
            string phoneNumber,
            string fromDate,
            string fromTime,
            string toTime)
        {
            try
            {
                var bookingWithComboDto = CreateBookingDto(courtId, comboId, fullName, phoneNumber, fromDate, fromTime, toTime);

                var result = await _mediator.Send(new BookingWithCombo.Command { Booking = bookingWithComboDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
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
