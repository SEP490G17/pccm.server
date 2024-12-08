using Application.Handler.Reviews;
using Moq;
using NUnit.Framework;
using Persistence;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pccm.UnitTest.Reviews
{
    public class DeleteReviewHandlerTests
    {
        private DataContext _context;
        private Delete.Handler _handler;

        [SetUp]
        public void Setup()
        {
            // Tạo DbContext với InMemoryDatabase
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Đặt tên cho DB trong bộ nhớ
                .Options;

            _context = new DataContext(options);
            _handler = new Delete.Handler(_context);  // Initialize handler với mock DataContext
        }

        [TearDown]
        public void TearDown()
        {
            // Xóa dữ liệu sau mỗi test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Handle_ReviewExists_ReturnsSuccess()
        {
            // Arrange: Tạo và thêm review vào DB In-Memory
            var reviewId = 1;
            var review = new Review { Id = reviewId };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var command = new Delete.Command { Id = reviewId };

            // Act: Gọi handler để xóa review
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert: Kiểm tra kết quả trả về
            Assert.That(result.IsSuccess, Is.True);
        }

    }
}
