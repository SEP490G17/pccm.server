using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Handler.CourtCombos;
using FluentAssertions;

namespace Pccm.UnitTest.Notifications
{
    [TestFixture]
    public class CreateCourtComboHandlerTests
    {
      private DataContext _context;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            // Configure InMemoryDatabase
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(options);

            // Configure AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CourtComboDto, CourtCombo>();
                cfg.CreateMap<CourtCombo, CourtComboDto>();
            });
            _mapper = config.CreateMapper();

            // Seed initial data
            _context.Courts.Add(new Court
            {
                Id = 1,
                CourtName = "Test Court",
                CourtCombos = new List<CourtCombo>
                {
                    new CourtCombo { Id = 1, DisplayName = "Old Combo 1", TotalPrice = 100, Duration = 60 },
                    new CourtCombo { Id = 2, DisplayName = "Old Combo 2", TotalPrice = 200, Duration = 90 }
                }
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            // Clear database after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Handle_ShouldUpdateCourtCombos_WhenValidRequestIsProvided()
        {
            // Arrange
            var handler = new CreateCourtCombo.Handler(_context, _mapper);

            var command = new CreateCourtCombo.Command
            {
                CourtId = 1,
                CourtComboCreateDtos = new List<CourtComboDto>
                {
                    new CourtComboDto { DisplayName = "New Combo 1", TotalPrice = 150, Duration = 70 },
                    new CourtComboDto { DisplayName = "New Combo 2", TotalPrice = 250, Duration = 120 }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            var court = await _context.Courts.Include(c => c.CourtCombos).FirstOrDefaultAsync(c => c.Id == 1);
            court.Should().NotBeNull();
            court.CourtCombos.Should().HaveCount(2);

            var updatedCombos = court.CourtCombos;
            updatedCombos.Should().ContainSingle(c => c.DisplayName == "New Combo 1" && c.TotalPrice == 150 && c.Duration == 70);
            updatedCombos.Should().ContainSingle(c => c.DisplayName == "New Combo 2" && c.TotalPrice == 250 && c.Duration == 120);
        }

        [Test]
        public async Task Handle_ShouldFail_WhenCourtDoesNotExist()
        {
            // Arrange
            var handler = new CreateCourtCombo.Handler(_context, _mapper);

            var command = new CreateCourtCombo.Command
            {
                CourtId = 99, // Non-existent CourtId
                CourtComboCreateDtos = new List<CourtComboDto>
                {
                    new CourtComboDto { DisplayName = "Combo 1", TotalPrice = 150, Duration = 70 }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Sân không tồn tại");
        }

        [Test]
        public async Task Handle_ShouldFail_WhenComboListIsEmpty()
        {
            // Arrange
            var handler = new CreateCourtCombo.Handler(_context, _mapper);

            var command = new CreateCourtCombo.Command
            {
                CourtId = 1,
                CourtComboCreateDtos = new List<CourtComboDto>() // Empty list
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Danh sách combo không được rỗng");
        }
    }

}

