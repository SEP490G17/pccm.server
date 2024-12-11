using Application.DTOs;
using Application.Handler.CourtPrices;
using AutoMapper;
using Domain.Entity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Pccm.UnitTest.CourtPrices
{
    [TestFixture]
    public class GetCourtPricesOfCourtHandlerTests
    {
        private DataContext _context;
        private IMapper _mapper;
        private GetCourtPricesOfCourt.Handler _handler;

        [SetUp]
        public void SetUp()
        {
            // Khởi tạo DataContext với InMemoryDatabase
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(options);

            // Cấu hình AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CourtPrice, CourtPriceResponseDto>();
            });
            _mapper = mapperConfig.CreateMapper();

            // Khởi tạo Handler
            _handler = new GetCourtPricesOfCourt.Handler(_context, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            // Dọn dẹp database sau mỗi test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Handle_CourtDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var query = new GetCourtPricesOfCourt.Query { CourtId = 1 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Sân không tồn tại");
        }

        [Test]
        public async Task Handle_CourtExists_ReturnsCourtPrices()
        {
            // Arrange
            var courtId = 1;

            // Thêm court vào InMemoryDatabase
            var court = new Court
            {
                Id = courtId,
                CourtName = "Test Court Name",
                DeleteAt = DateTime.UtcNow // Đặt giá trị khác null cho DeleteAt
            };
            _context.Courts.Add(court);

            // Thêm court prices vào InMemoryDatabase
            var courtPrices = new List<CourtPrice>
                {
                    new CourtPrice { Id = 1, Court = court, Price = 100m },
                    new CourtPrice { Id = 2, Court = court, Price = 200m }
                };
            _context.CourtPrices.AddRange(courtPrices);

            await _context.SaveChangesAsync();

            // Tạo query
            var query = new GetCourtPricesOfCourt.Query { CourtId = courtId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Select(cp => cp.Price).Should().Contain(new[] { 100m, 200m });
        }

        [Test]
        public async Task Handle_CourtIsDeleted_ReturnsFailure()
        {
            // Arrange
            var courtId = 1;

            // Thêm court bị xóa vào InMemoryDatabase
            var court = new Court { Id = courtId, DeleteAt = System.DateTime.UtcNow, CourtName = "Test Court Name" };
            _context.Courts.Add(court);

            await _context.SaveChangesAsync();

            var query = new GetCourtPricesOfCourt.Query { CourtId = 123 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Sân không tồn tại");
        }
    }
}