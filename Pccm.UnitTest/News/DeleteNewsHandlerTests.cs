using Moq;
using NUnit.Framework;
using Persistence;
using Application.Events;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entity;
using System.Linq;
using Domain.Enum;
using MediatR;

namespace Pccm.UnitTest.News
{
    [TestFixture]
    public class DeleteNewsHandlerTests
    {
        private DataContext _context;
        private Delete.Handler _handler;

        [SetUp]
        public void Setup()
        {
            // Use InMemoryDatabase for testing
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            // Initialize the in-memory context
            _context = new DataContext(options);

            // Clear existing data to avoid duplicates
            _context.NewsBlogs.RemoveRange(_context.NewsBlogs);
            _context.SaveChanges();

            // Seed the in-memory database with sample data
            _context.NewsBlogs.AddRange(
                new NewsBlog
                {
                    Id = 1,
                    Title = "News 1",
                    Thumbnail = "thumbnail1.jpg",
                    Description = "Description 1",
                    Content = "Content 1",
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Location = "Location 1",
                    Status = BannerStatus.Display,
                    Tags = new string[] { "Tag1", "Tag2" },
                    NewsLogs = new List<NewsLog>()
                },
                new NewsBlog
                {
                    Id = 2,
                    Title = "News 2",
                    Thumbnail = "thumbnail2.jpg",
                    Description = "Description 2",
                    Content = "Content 2",
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Location = "Location 2",
                    Status = BannerStatus.Display,
                    Tags = new string[] { "Tag3", "Tag4" },
                    NewsLogs = new List<NewsLog>()
                }
            );
            _context.SaveChanges();

            // Initialize the handler with the context
            _handler = new Delete.Handler(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the in-memory context after each test
            _context.Dispose();
        }

        [Test]
        public async Task Should_Delete_Existing_NewsBlog()
        {
            // Arrange: Command to delete NewsBlog with Id = 1
            var command = new Delete.Command { Id = 1 };

            // Act: Execute the delete command
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert: Check that the result is success
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(Unit.Value, result.Value);

            // Ensure that the NewsBlog with Id = 1 is deleted
            var newsBlog = await _context.NewsBlogs.FindAsync(1);
            Assert.IsNull(newsBlog); // The NewsBlog should no longer exist
        }

       
    }
}
