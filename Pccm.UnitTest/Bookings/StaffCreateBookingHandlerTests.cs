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
    public class StaffCreateBookingHandlerTests
    {
        private DbContextOptions<DataContext> _dbContextOptions;
        private DataContext _dbContext;
        private IMapper _mapper;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private StaffCreate.Handler _handler;

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

            _handler = new StaffCreate.Handler(
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
        public async Task Handle_Should_Return_Failure_When_Court_Not_Found()
        {
           var command = new StaffCreate.Command
            {
                Booking = new BookingInputDto
                {
                    CourtId = 1,
                    StartTime = DateTime.Parse("2025-12-05 03:45:00.000"),
                    EndTime = DateTime.Parse("2025-12-05 07:45:00.000"),
                    PhoneNumber = "0987654321",
                    FullName = "John Doe"
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Sân không tồn tại hoặc đã dừng hoạt động");
        }


        [Test]
        public async Task Handle_Should_Create_Booking_When_No_Conflicts()
        {

            // 1️⃣ Arrange: Setup database context, user, court, and existing combo booking
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new DataContext(options);
          
            var config = new MapperConfiguration(cfg =>
            {
                // Add your mapping profile here
                cfg.AddProfile<BookingMappingProfile>(); 
            });
            
            _mapper = config.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

            // // 2️⃣ Create an existing **confirmed recurring booking**
             await context.SaveChangesAsync();

          var command = new StaffCreate.Command
            {
                Booking = new BookingInputDto
                {
                    CourtId = 1,
                    StartTime = DateTime.UtcNow.Date.AddHours(30),
                    EndTime = DateTime.UtcNow.Date.AddHours(41),
                    PhoneNumber = "0987654321",
                    FullName = "John Doe"
                }
            };

            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);

            var handler = new StaffCreate.Handler(context, _mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Conflicts_With_Recurring_Bookings()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new DataContext(options);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BookingMappingProfile>(); 
            });
            
            _mapper = config.CreateMapper();

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
                UntilTime = DateTime.Parse("2026-12-07 07:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

           var command = new StaffCreate.Command
            {
                Booking = new BookingInputDto
                {
                    CourtId = 1,
                    StartTime = DateTime.Parse("2026-11-06 04:45:00.000"),
                    EndTime = DateTime.Parse("2026-11-06 06:45:00.000"),
                    PhoneNumber = "0987654321",
                    FullName = "John Doe"
                }
            };

            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);


            var handler = new StaffCreate.Handler(context, _mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            result.Error.Should().Be("Trùng với một số lịch đặt theo combo đã được confirm trước đó");
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_With_Before_CurrentDay_Bookings()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new DataContext(options);
          
            var config = new MapperConfiguration(cfg =>
            {
            
                cfg.AddProfile<BookingMappingProfile>();
            });
            
            _mapper = config.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2024-12-05 14:45:00.000"),
                EndTime = DateTime.Parse("2024-12-05 20:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

             var command = new StaffCreate.Command
            {
                Booking = new BookingInputDto
                {
                    CourtId = 1,
                    StartTime = DateTime.Parse("2024-12-04 14:45:00.000"),
                    EndTime = DateTime.Parse("2024-12-04 14:45:00.000"),
                    PhoneNumber = "0987654321",
                    FullName = "John Doe"
                }
            };

            // 4️⃣ Create the handler and run the request
            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);


            var handler = new StaffCreate.Handler(context, _mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            result.Error.Should().Be("Không thể đặt lịch ngày trước ngày hiện tại");
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Conflicts_With_SingleDay_Bookings()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new DataContext(options);
       
            var config = new MapperConfiguration(cfg =>
            {

                cfg.AddProfile<BookingMappingProfile>(); 
            });
            
            _mapper = config.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "Hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

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

            var command = new StaffCreate.Command
            {
                Booking = new BookingInputDto
                {
                    CourtId = 1,
                    StartTime = DateTime.Parse("2024-12-20 03:45:00.000"),
                    EndTime = DateTime.Parse("2024-12-20 07:45:00.000"),
                    PhoneNumber = "0987654321",
                    FullName = "John Doe"
                }
            };

            var userAccessorMock = new Mock<IUserAccessor>();
            userAccessorMock.Setup(x => x.GetUserName()).Returns(user.UserName);

            var handler = new StaffCreate.Handler(context, _mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Trùng với một số lịch đặt theo ngày đã được confirm trước đó", result.Error); // Adjust error message if needed
        }
    }
}
