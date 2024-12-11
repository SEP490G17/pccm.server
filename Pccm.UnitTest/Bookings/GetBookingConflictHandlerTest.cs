using Application.Core;
using Application.DTOs;
using Application.Handler.Bookings;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Entity;
using Domain.Enum;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class GetBookingConflictHandlerTest
    {
        private DbContextOptions<DataContext> _dbContextOptions;
        private Mock<DataContext> _mockContext;
        private Mock<IMapper> _mockMapper;
        private DataContext _dbContext;
        private IMapper _mapper;
        private Mock<IUserAccessor> _mockUserAccessor;
        private Mock<UserManager<AppUser>> _mockUserManager;
        private GetBookingConflict.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Initialize dependencies
            _dbContext = new DataContext(_dbContextOptions);
            _mockMapper = new Mock<IMapper>();
            _mockUserAccessor = new Mock<IUserAccessor>();
            _mockUserManager = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null
            );

            var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new MappingProfile());
                });
            _mapper = configuration.CreateMapper();


            // 1️⃣ Create an authenticated user and set it in the HttpContext
            var userClaims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "administrator"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "ManagerCourtCluster")
            };
            var identity = new System.Security.Claims.ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            // 2️⃣ Add a user to the in-memory database
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

            // 3️⃣ Mock UserManager to find the user by username
            _mockUserManager.Setup(x => x.FindByNameAsync("administrator")).ReturnsAsync(user);

            // 4️⃣ Mock UserManager to return roles for the user
            var roles = new List<string> { "Admin", "ManagerCourtCluster" };
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            // 5️⃣ Mock UserAccessor to return the current username
            _mockUserAccessor.Setup(x => x.GetUserName()).Returns("administrator");

            // 6️⃣ Initialize the handler
            _handler = new GetBookingConflict.Handler(
                _dbContext,
                _mockMapper.Object,
                _mockUserAccessor.Object,
                _mockUserManager.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup the in-memory database
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_User_Not_Found()
        {
            // Arrange
            _mockUserAccessor.Setup(x => x.GetUserName()).Returns("invaliduser");
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);

            var command = new GetBookingConflict.Command
            {
                Booking = new BookingConflictDto { CourtId = 1, BookingId = 1 }
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
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new DataContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2025-12-05 03:45:00.000"),
                EndTime = DateTime.Parse("2025-12-05 07:45:00.000"),
                UntilTime = DateTime.Parse("2026-12-05 03:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

            var request = new GetBookingConflict.Command
            {
                Booking = new BookingConflictDto
                {
                    CourtId = 22,
                    FromDate = DateTime.Parse("2025-11-01"),
                    FromTime = TimeOnly.Parse("10:00"),
                    ToTime = TimeOnly.Parse("12:00"),
                    BookingId = 1
                }
            };

            var command = new GetBookingConflict.Command
            {
                Booking = new BookingConflictDto { CourtId = 1, BookingId = 1 }
            };

            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);

            var handler = new GetBookingConflict.Handler(context, mapper, userAccessorMock.Object, _mockUserManager.Object);
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Sân không tồn tại");
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Not_Found()
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

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);
            var courtCluster = new CourtCluster
            {
                Id = 1,
                CourtClusterName = "Court Cluster 1",
                Province = "Province",
                ProvinceName = "Province Name",
                District = "District",
                DistrictName = "District Name",
                Ward = "Ward",
                WardName = "Ward Name",
                Address = "123 Street, City",
                CreatedAt = DateTime.Now
            };

            var court = new Court
            {
                Id = 1,
                CourtName = "Court 1",
                CourtClusterId = courtCluster.Id,
                Status = CourtStatus.Available,
                CourtCluster = courtCluster,
            };

            var courtPrice = new CourtPrice
            {
                Id = 1,
                DisplayName = "Gio sang",
                Court = court,
                FromTime = new TimeOnly(7, 0),
                ToTime = new TimeOnly(12, 0),
                Price = 500000m
            };

            await context.Courts.AddAsync(court);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2025-12-05 03:45:00.000"),
                EndTime = DateTime.Parse("2025-12-05 07:45:00.000"),
                UntilTime = null,
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };
            await context.CourtPrices.AddAsync(courtPrice);
            await context.CourtClusters.AddAsync(courtCluster);
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

            var request = new GetBookingConflict.Command
            {
                Booking = new BookingConflictDto
                {
                    CourtId = 1,
                    FromDate = DateTime.Parse("2025-11-01"),
                    FromTime = TimeOnly.Parse("10:00"),
                    ToTime = TimeOnly.Parse("12:00"),
                    BookingId = 122
                }
            };

            // 4️⃣ Create the handler and run the request
            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<string> { "User" });

            var handler = new GetBookingConflict.Handler(context, mapper, userAccessorMock.Object, _mockUserManager.Object);
            var result = await handler.Handle(request, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Booking không tồn tại");
        }

        [Test]
        public async Task Handle_Should_Return_Conflicts_When_There_Are_Bookings_With_Conflicts()
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

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);
            var courtCluster = new CourtCluster
            {
                Id = 1,
                CourtClusterName = "Court Cluster 1",
                Province = "Province",
                ProvinceName = "Province Name",
                District = "District",
                DistrictName = "District Name",
                Ward = "Ward",
                WardName = "Ward Name",
                Address = "123 Street, City",
                CreatedAt = DateTime.Now
            };

            var court = new Court
            {
                Id = 1,
                CourtName = "Court 1",
                CourtClusterId = courtCluster.Id,
                Status = CourtStatus.Available,
                CourtCluster = courtCluster,
            };

            var courtPrice = new CourtPrice
            {
                Id = 1,
                DisplayName = "Gio sang",
                Court = court,
                FromTime = new TimeOnly(7, 0),
                ToTime = new TimeOnly(12, 0),
                Price = 500000m
            };

            await context.Courts.AddAsync(court);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2025-12-05 03:45:00.000"),
                EndTime = DateTime.Parse("2025-12-05 07:45:00.000"),
                UntilTime = DateTime.Parse("2026-12-05 07:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };
            await context.CourtPrices.AddAsync(courtPrice);
            await context.CourtClusters.AddAsync(courtCluster);
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

            var request = new GetBookingConflict.Command
            {
                Booking = new BookingConflictDto
                {
                    CourtId = 1,
                    FromDate = DateTime.Parse("2026-11-01"),
                    FromTime = TimeOnly.Parse("04:00"),
                    ToTime = TimeOnly.Parse("05:00"),
                    BookingId = 1
                }
            };

            // 4️⃣ Create the handler and run the request
            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<string> { "User" });

            var handler = new GetBookingConflict.Handler(context, mapper, userAccessorMock.Object, _mockUserManager.Object);
            var result = await handler.Handle(request, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();
        }

    }
}