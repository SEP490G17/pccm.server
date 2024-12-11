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
    public class DenyBookingHandlerTests
    { 
         private DbContextOptions<DataContext> _dbContextOptions;
        private DataContext _dbContext;
        private IMapper _mapper;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private DenyBooking.Handler _handler;

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

            _handler = new DenyBooking.Handler(
                _dbContext,
                _mockMapper.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Handle_BookingNotFound_ReturnsFailure()
        {
            // Arrange
            var command = new DenyBooking.Command
            {
                Id = 1
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Booking không được tìm thấy.");
        }

        [Test]
        public async Task Handle_BookingAlreadyProcessed_ReturnsFailure()
        {
            // Arrange
            var command = new DenyBooking.Command
            {
                Id = 1
            };

            var booking = new Booking
            {
                Id = 1,
                Status = BookingStatus.Confirmed, 
                IsSuccess = true,
                PhoneNumber = "0939393393"
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Booking đã được xác nhận/từ chối trước đó.");
        }

        [Test]
        public async Task Handle_BookingPending_AndDeclined_ReturnsSuccess()
        {
            // Arrange
            var command = new DenyBooking.Command
            {
                Id = 1
            };

            var booking = new Booking
            {
                Id = 1,
                Status = BookingStatus.Pending, 
                IsSuccess = false,
                PhoneNumber = "0040430033"
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();

            var bookingDto = new BookingDtoV2
            {
                Id = 1
            };

            _mockMapper.Setup(m => m.Map<BookingDtoV2>(It.IsAny<Booking>()))
                .Returns(bookingDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(1);
            booking.Status.Should().Be(BookingStatus.Declined);
        }

    }
}
