using Moq;
using NUnit.Framework;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Handler.Bookings;
using Microsoft.AspNetCore.Identity;
using Domain;
using FluentAssertions;
using Moq.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class BookingWithComboHandlerTests
    {
        private DbContextOptions<DataContext> _dbContextOptions;
        private DataContext _dbContext;
        private IMapper _mapper;
        private Mock<IMapper> _mockMapper;
        private Mock<IUserAccessor> _mockUserAccessor;
        private Mock<UserManager<AppUser>> _mockUserManager;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private BookingWithCombo.Handler _handler;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database for each test
                .Options;

            // Initialize dependencies
            _dbContext = new DataContext(_dbContextOptions);
            _mockMapper = new Mock<IMapper>();
            _mockUserAccessor = new Mock<IUserAccessor>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _mockUserManager = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null
            );

            var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new MappingProfile()); // Your AutoMapper profile here
                });
            _mapper = configuration.CreateMapper();


            var userClaims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "administrator"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "ManagerCourtCluster")
            };
            var identity = new System.Security.Claims.ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var user = new AppUser
            {
                Id = "d6341ccf-1a22-426c-83bd-21f3f63cd83f",
                UserName = "adminstrator",
                FirstName = "Alexandros",
                LastName = "Papadopoulos",
                Email = "adminstrator@test.com"
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            _mockUserManager.Setup(x => x.FindByNameAsync("administrator")).ReturnsAsync(user);

            var roles = new List<string> { "Admin", "ManagerCourtCluster" };
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            _mockUserAccessor.Setup(x => x.GetUserName()).Returns("administrator");

            _handler = new BookingWithCombo.Handler(
                _dbContext,
                _mockMapper.Object,
                _mockUserAccessor.Object,
                _mockUserManager.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_User_Not_Found()
        {
            _mockUserAccessor.Setup(u => u.GetUserName()).Returns("non_existing_user");

            var command = new BookingWithCombo.Command
            {
                Booking = new BookingWithComboDto
                {
                    CourtId = 1,
                    FromDate = DateTime.UtcNow,
                    FromTime = new TimeOnly(10, 0),
                    ToTime = new TimeOnly(11, 0),
                    PhoneNumber = "0987654321",
                    FullName = "John Doe"
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Username không tồn tại");
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Court_Not_Found()
        {
            _mockUserAccessor.Setup(u => u.GetUserName()).Returns("existing_user");
            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());

            var command = new BookingWithCombo.Command
            {
                Booking = new BookingWithComboDto
                {
                    CourtId = 999, // Invalid court ID
                    FromDate = DateTime.UtcNow,
                    FromTime = new TimeOnly(10, 0),
                    ToTime = new TimeOnly(11, 0),
                    PhoneNumber = "0987654321",
                    FullName = "John Doe"
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Sân không tồn tại");
        }

        [Test]
        public async Task Handle_Should_Create_BookingwithCombo_When_No_Conflicts()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new DataContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "Hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

            var combo = new CourtCombo
            {
                Id = 1,
                Duration = 1, // 1 month
                TotalPrice = 500000 // Example price for combo
            };
            await context.CourtCombos.AddAsync(combo);
            await context.SaveChangesAsync();

            // 2️⃣ Create an existing confirmed booking (single-day booking)
            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2024-12-20 14:45:00.000"),
                EndTime = DateTime.Parse("2024-12-20 20:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994",
                UntilTime = null
            };
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

            var request = new BookingWithCombo.Command
            {
                Booking = new BookingWithComboDto
                {
                    CourtId = court.Id,
                    ComboId = combo.Id,
                    FromDate = DateTime.Parse("2024-12-20"),
                    FromTime = TimeOnly.Parse("15:00"),
                    ToTime = TimeOnly.Parse("18:00"),
                    PhoneNumber = "0123456789",
                    FullName = "Test User"
                }
            };

            // 4️⃣ Mock user accessor and user manager
            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);

            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<string> { "User" });

            var handler = new BookingWithCombo.Handler(context, mapper, userAccessorMock.Object, _mockUserManager.Object);
            var result = await handler.Handle(request, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Conflicts_With_SingleDay_Bookings()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new DataContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "Hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            var courtCombo = new CourtCombo
            {
                Id = 1,
                DisplayName = "Court 1 Combo",
                CourtId = 1,
                TotalPrice = 100.00m,
                Duration = 60
            };
            await context.Courts.AddAsync(court);
            await context.CourtCombos.AddAsync(courtCombo);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2024-12-20 03:45:00.000"),
                EndTime = DateTime.Parse("2024-12-20 07:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994",
                UntilTime = null
            };
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

            var request = new BookingWithCombo.Command
            {
                Booking = new BookingWithComboDto
                {
                    CourtId = court.Id,
                    ComboId = 1,
                    FromDate = DateTime.Parse("2024-12-20"),
                    FromTime = TimeOnly.Parse("11:00"),
                    ToTime = TimeOnly.Parse("12:00"),
                    PhoneNumber = "0123456789",
                    FullName = "Test User"
                }
            };

            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);

            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<string> { "User" });

            var handler = new BookingWithCombo.Handler(context, mapper, userAccessorMock.Object, _mockUserManager.Object);
            var result = await handler.Handle(request, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Trùng với một số lịch đặt theo ngày đã được confirm trước đó", result.Error);
        }

        //     // 4️⃣ Create the handler and run the request
    }
}
