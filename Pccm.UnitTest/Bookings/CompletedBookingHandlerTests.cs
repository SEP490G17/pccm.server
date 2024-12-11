using Moq;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Handler.Bookings;
using Microsoft.AspNetCore.Identity;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class CompletedBookingHandlerTests
    {
        private DbContextOptions<DataContext> _dbContextOptions;
        private DataContext _dbContext;
        private IMapper _mapper;
        private Mock<IMapper> _mockMapper;
        private Mock<IUserAccessor> _mockUserAccessor;
        private Mock<UserManager<AppUser>> _mockUserManager;
        private CompletedBooking.Handler _handler;

        [SetUp]
        public void SetUp()
        {
            // Configure in-memory database options
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database for each test
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
                    cfg.AddProfile(new MappingProfile()); // Your AutoMapper profile here
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
            _handler = new CompletedBooking.Handler(
                _dbContext,
                _mockMapper.Object
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

            var request = new CompletedBooking.Command
            {
                Id = 2
            };

            // 4️⃣ Create the handler and run the request
            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<string> { "User" });

            var handler = new CompletedBooking.Handler(context, mapper);
            var result = await handler.Handle(request, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Booking không được tìm thấy");
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Not_Payment()
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

            var newPayment = new Payment
            {
                Amount = 1000000m,
                PaymentMethod = Domain.Enum.PaymentMethod.VNPay,
                PaymentUrl = "https://vnpay.vn/payment-url",
                Status = Domain.Enum.PaymentStatus.Pending,
                TransactionRef = "VN1234567890",
                CreatedAt = DateTime.UtcNow,
                PaidAt = null,
                BookingId = 1,
                OrderId = null
            };
            await context.CourtPrices.AddAsync(courtPrice);
            await context.Payments.AddAsync(newPayment);
            await context.CourtClusters.AddAsync(courtCluster);
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

            var request = new CompletedBooking.Command
            {
                Id = 1
            };

            // 4️⃣ Create the handler and run the request
            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<string> { "User" });

            var handler = new CompletedBooking.Handler(context, mapper);
            var result = await handler.Handle(request, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Booking chưa được thanh toán");
        }


        [Test]
        public async Task Handle_Should_Return_Failure_When_Order_Not_Payment()
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

            var newPayment = new Payment
            {
                Id = 1,
                Amount = 1000000m,
                PaymentMethod = Domain.Enum.PaymentMethod.VNPay,
                PaymentUrl = "https://vnpay.vn/payment-url",
                Status = Domain.Enum.PaymentStatus.Success,
                TransactionRef = "VN1234567890",
                CreatedAt = DateTime.UtcNow,
                PaidAt = null,
                BookingId = 1,
                OrderId = 1
            };

            var newOrder = new Order
            {
                Id = 1,
                CreatedBy = 1, 
                CreatedAt = DateTime.UtcNow, 
                TotalAmount = 1500000m,
                Discount = 0.1f, 
                IsOpen = true, 
                BookingId = 1, 
            };

            await context.CourtPrices.AddAsync(courtPrice);
            await context.Orders.AddAsync(newOrder);
            await context.Payments.AddAsync(newPayment);
            await context.CourtClusters.AddAsync(courtCluster);
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

            var request = new CompletedBooking.Command
            {
                Id = 1
            };

            // 4️⃣ Create the handler and run the request
            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<string> { "User" });

            var handler = new CompletedBooking.Handler(context, mapper);
            var result = await handler.Handle(request, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccess);
        }



    }
}
