using Moq;
using NUnit.Framework;
using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Persistence;
using Application.Handler.Bookings;
using System.Linq.Expressions;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class AcceptedBookingHandlerTests
    {
        private DbContextOptions<DataContext> _dbContextOptions;
        private DataContext _dbContext;
        private Mock<IMapper> _mockMapper;
        private AcceptedBooking.Handler _handler;

        [SetUp]
        public void SetUp()
        {
            // Setup the In-Memory Database
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // Use a unique name for each test
                .Options;

            _dbContext = new DataContext(_dbContextOptions);
            _mockMapper = new Mock<IMapper>();

            _handler = new AcceptedBooking.Handler(_dbContext, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Xóa dữ liệu sau mỗi test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Not_Found()
        {
            // Arrange: Create a CourtCluster with Courts and an expired Booking
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
                CourtName = "Court 1",
                CourtClusterId = courtCluster.Id,
                Status = CourtStatus.Available,
                CourtCluster = courtCluster,
            };

            var expiredBooking = new Booking
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(530), // Expired booking
                EndTime = DateTime.UtcNow.AddHours(690).AddMinutes(30),
                Status = BookingStatus.Pending,
                Court = court,
                TotalPrice = 1500,
                PhoneNumber = "0987654321",
                FullName = "John Doe"
            };

            _dbContext.CourtClusters.Add(courtCluster);
            _dbContext.Courts.Add(court);
            _dbContext.Bookings.Add(expiredBooking);
            await _dbContext.SaveChangesAsync();
            // Arrange
            var command = new AcceptedBooking.Command { Id = 111 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Booking không được tìm thấy");
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Expired()
        {
            // Arrange: Create a CourtCluster with Courts and an expired Booking
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
                CourtName = "Court 1",
                CourtClusterId = courtCluster.Id,
                Status = CourtStatus.Available,
                CourtCluster = courtCluster,
            };

            var expiredBooking = new Booking
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(-1), // Expired booking
                EndTime = DateTime.UtcNow.AddHours(-1).AddMinutes(30),
                Status = BookingStatus.Pending,
                Court = court,
                TotalPrice = 1500,
                PhoneNumber = "0987654321",
                FullName = "John Doe"
            };

            _dbContext.CourtClusters.Add(courtCluster);
            _dbContext.Courts.Add(court);
            _dbContext.Bookings.Add(expiredBooking);
            await _dbContext.SaveChangesAsync();

            var command = new AcceptedBooking.Command { Id = 1 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Lịch đặt đã quá hạn");
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Conflicts_With_Existing_Confirmed_Booking()
        {
            // Arrange: Create Court and CourtCluster with an existing confirmed booking
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
                CourtName = "Court 1",
                CourtClusterId = courtCluster.Id,
                Status = CourtStatus.Available,
                CourtCluster = courtCluster,
            };

            var existingBooking = new Booking
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2),
                Status = BookingStatus.Confirmed,
                Court = court,
                TotalPrice = 1500,
                PhoneNumber = "0987654321",
                FullName = "Jane Doe"
            };

            var newBooking = new Booking
            {
                Id = 2,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2),
                Status = BookingStatus.Pending,
                Court = court,
                TotalPrice = 1500,
                PhoneNumber = "0912345678",
                FullName = "John Doe"
            };

            _dbContext.CourtClusters.Add(courtCluster);
            _dbContext.Courts.Add(court);
            _dbContext.Bookings.Add(existingBooking);
            _dbContext.Bookings.Add(newBooking);
            await _dbContext.SaveChangesAsync();

            var command = new AcceptedBooking.Command { Id = 2 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Trùng với một số lịch đặt theo ngày đã được confirm trước đó");
        }

        [Test]
        public async Task Handle_Should_Update_Booking_Status_When_Valid()
        {
            // Arrange: Create Court and CourtCluster with valid booking
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
                CourtName = "Court 1",
                CourtClusterId = courtCluster.Id,
                Status = CourtStatus.Available,
                CourtCluster = courtCluster,
            };

            var booking = new Booking
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2),
                Status = BookingStatus.Pending,
                Court = court,
                TotalPrice = 1500,
                PhoneNumber = "0987654321",
                FullName = "John Doe"
            };

            var payment = new Payment { Amount = 1500, Status = PaymentStatus.Pending, BookingId = booking.Id };

            _dbContext.CourtClusters.Add(courtCluster);
            _dbContext.Courts.Add(court);
            _dbContext.Bookings.Add(booking);
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();

            var command = new AcceptedBooking.Command { Id = 1 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            booking.Status.Should().Be(BookingStatus.Confirmed);
            payment.Status.Should().Be(PaymentStatus.Pending);  // Payment remains pending until processed
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Conflicts_With_Single_Day_Bookings()
        {
            // Arrange: Create Court and CourtCluster with recurring booking conflicts
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
                CourtName = "Court 1",
                CourtClusterId = courtCluster.Id,
                Status = CourtStatus.Available,
                CourtCluster = courtCluster,
            };

            var recurringBooking = new Booking
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(1), // Overlaps with a new booking
                EndTime = DateTime.UtcNow.AddHours(2),
                Status = BookingStatus.Confirmed,
                Court = court,
                TotalPrice = 1500,
                PhoneNumber = "0987654321",
                FullName = "John Doe"
            };

            var conflictingBooking = new Booking
            {
                Id = 2,
                StartTime = DateTime.UtcNow.AddHours(1), // Conflicts with the recurring booking
                EndTime = DateTime.UtcNow.AddHours(2),
                Status = BookingStatus.Pending,
                Court = court,
                TotalPrice = 1500,
                PhoneNumber = "0912345678",
                FullName = "Jane Doe"
            };

            _dbContext.CourtClusters.Add(courtCluster);
            _dbContext.Courts.Add(court);
            _dbContext.Bookings.Add(recurringBooking);
            _dbContext.Bookings.Add(conflictingBooking);
            await _dbContext.SaveChangesAsync();

            var command = new AcceptedBooking.Command { Id = 2 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Trùng với một số lịch đặt theo ngày đã được confirm trước đó");
        }

              [Test]
        public async Task Handle_Should_Return_Failure_When_Booking_Conflicts_With_Recurring_Bookings()
        {
            // Arrange: Create Court and CourtCluster with recurring booking conflicts
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
                CourtName = "Court 1",
                CourtClusterId = courtCluster.Id,
                Status = CourtStatus.Available,
                CourtCluster = courtCluster,
            };

            var recurringBooking = new Booking
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(1), // Overlaps with a new booking
                EndTime = DateTime.UtcNow.AddHours(2),
                Status = BookingStatus.Confirmed,
                Court = court,
                UntilTime = DateTime.UtcNow.AddHours(2000),
                TotalPrice = 1500,
                PhoneNumber = "0987654321",
                FullName = "John Doe"
            };

            var conflictingBooking = new Booking
            {
                Id = 2,
                StartTime = DateTime.UtcNow.AddHours(50), // Conflicts with the recurring booking
                EndTime = DateTime.UtcNow.AddHours(72),
                Status = BookingStatus.Pending,
                Court = court,
                TotalPrice = 1500,
                PhoneNumber = "0912345678",
                FullName = "Jane Doe"
            };

            _dbContext.CourtClusters.Add(courtCluster);
            _dbContext.Courts.Add(court);
            _dbContext.Bookings.Add(recurringBooking);
            _dbContext.Bookings.Add(conflictingBooking);
            await _dbContext.SaveChangesAsync();

            var command = new AcceptedBooking.Command { Id = 2 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Trùng với một số lịch đặt theo combo đã được confirm trước đó");
        }

    }
}
