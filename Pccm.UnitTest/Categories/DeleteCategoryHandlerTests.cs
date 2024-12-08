using Application.Handler.Categories;
using Application.Core;
using Domain.Entity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace Pccm.UnitTest.Categories
{
    public class DeleteCategoryHandlerTests
    {
       private DataContext _context;
        private Delete.Handler _handler;

        [SetUp]
        public async Task SetUp()
        {
            // Thiết lập DbContext với InMemoryDatabase
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(options);

            // Thêm dữ liệu giả vào cơ sở dữ liệu
            _context.Categories.Add(new Category { Id = 1, CategoryName = "Category 1" });
            _context.Categories.Add(new Category { Id = 2, CategoryName = "Category 2" });

            await _context.SaveChangesAsync();

            // Khởi tạo handler
            _handler = new Delete.Handler(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Xóa toàn bộ cơ sở dữ liệu sau mỗi test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Handle_CategoryExists_ReturnsSuccess()
        {
            // Arrange
            var command = new Delete.Command { Id = 1 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _context.Categories.Find(1).Should().BeNull(); // Kiểm tra xem category đã bị xóa
        }

        [Test]
        public async Task Handle_CategoryDoesNotExist_ReturnsNull()
        {
            // Arrange
            var command = new Delete.Command { Id = 999 }; // ID không tồn tại

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

       
    }
}
