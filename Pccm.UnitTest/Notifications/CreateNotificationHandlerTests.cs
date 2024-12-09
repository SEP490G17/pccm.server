using Application.Handler.Notifications;
using Application.DTOs;
using AutoMapper;
using Domain;
using Domain.Entity;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace Pccm.UnitTest.Notifications
{
    [TestFixture]
    public class CreateNotificationHandlerTests
    {
        private DataContext _context;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            // Cấu hình InMemoryDatabase
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(options);

            // Cấu hình AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Notification, NotificationDto>();
            });
            _mapper = config.CreateMapper();
        }

        [TearDown]
        public void TearDown()
        {
            // Xóa dữ liệu sau mỗi test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Handle_ShouldCreateNotification_WhenValidRequestIsProvided()
        {
            // Arrange
            var handler = new CreateNotification.Handler(_context, _mapper);

            var command = new CreateNotification.Command
            {
                Title = "Test Title",
                Message = "Test Message",
                Type = NotificationType.Booking,
                Url = "http://example.com",
                AppUser = new AppUser
                {
                    Id = "b6341ccf-1a22-426c-83bd-21f3f63cd83f",
                    UserName = "adminstrator",
                    FirstName = "Alexandros",
                    LastName = "Papadopoulos",
                    Email = "adminstrator@test.com"
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Title", result.Title);
            Assert.AreEqual("Test Message", result.Message);
            Assert.AreEqual(NotificationType.Booking, result.Type);

            // Kiểm tra dữ liệu trong database
            var notificationInDb = await _context.Notifications.FirstOrDefaultAsync();
            Assert.IsNotNull(notificationInDb);
            Assert.AreEqual("Test Title", notificationInDb.Title);
        }
    }

}

