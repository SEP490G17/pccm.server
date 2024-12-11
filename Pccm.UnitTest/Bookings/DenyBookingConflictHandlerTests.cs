using Moq;
using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Handler.Bookings;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http;


namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class DenyBookingConflictHandlerTests
    {
        private DbContextOptions<DataContext> _dbContextOptions;
        private DataContext _dbContext;
        private IMapper _mapper;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private DenyBookingConflict.Handler _handler;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database for each test
                .Options;

            // Initialize dependencies
            _dbContext = new DataContext(_dbContextOptions);
            _mockMapper = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

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

            var roles = new List<string> { "Admin", "ManagerCourtCluster" };

            _handler = new DenyBookingConflict.Handler(
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
        public async Task Handle_NoBookingsToDecline_ReturnsFailure()
        {
            // Arrange
            var command = new DenyBookingConflict.Command
            {
                Id = new List<int> { 1, 2 }
            };

            _dbContext.Bookings.AddRange(new List<Booking>());
            _dbContext.SaveChanges();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Không có booking nào cần xóa hoặc đã được xác nhận/từ chối trước đó.");
        }

        [Test]
        public async Task Handle_BookingsFound_AndDeclined_ReturnsSuccess()
        {
            // Arrange
            var command = new DenyBookingConflict.Command
            {
                Id = new List<int> { 1, 2 }
            };

            var bookings = new List<Booking>
            {
                new Booking { Id = 1, Status = BookingStatus.Pending, PhoneNumber = "09933939398", IsSuccess = false },
                new Booking { Id = 2, Status = BookingStatus.Pending, PhoneNumber = "09933939398", IsSuccess = false }
            };

            _dbContext.Bookings.AddRange(bookings);
            _dbContext.SaveChanges();

            var bookingDtos = new List<BookingDtoV2>
            {
                new BookingDtoV2 { Id = 1 },
                new BookingDtoV2 { Id = 2 }
            };

            _mockMapper.Setup(m => m.Map<List<BookingDtoV2>>(It.IsAny<List<Booking>>()))
                .Returns(bookingDtos);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            bookings[0].Status.Should().Be(BookingStatus.Declined);
            bookings[1].Status.Should().Be(BookingStatus.Declined);
        }

        // [Test]
        // public async Task Handle_SaveChangesFails_ReturnsFailure()
        // {
        //     // Arrange
        //     var command = new DenyBookingConflict.Command
        //     {
        //         Id = new List<int> { 1, 2 }
        //     };

        //     var bookings = new List<Booking>
        //     {
        //         new Booking { Id = 1, Status = BookingStatus.Pending, IsSuccess = false },
        //         new Booking { Id = 2, Status = BookingStatus.Pending, IsSuccess = false }
        //     };

        //     _dbContext.Bookings.AddRange(bookings);
        //     _dbContext.SaveChanges();

        //     _dbContext.Setup(x => x.SaveChangesAsync(CancellationToken.None))
        //         .ReturnsAsync(0); // Simulate failed save

        //     // Act
        //     var result = await _handler.Handle(command, CancellationToken.None);

        //     // Assert
        //     result.IsSuccess.Should().BeFalse();
        //     result.Error.Should().Be("Xóa lịch thất bại.");
        // }
    }
}
