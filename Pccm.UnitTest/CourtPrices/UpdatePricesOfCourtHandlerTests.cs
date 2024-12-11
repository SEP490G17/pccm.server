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
public class UpdatePricesOfCourtHandlerTests
{
    private DataContext _context;
    private IMapper _mapper;
    private UpdatePricesOfCourt.Handler _handler;

    [SetUp]
    public void Setup()
    {
        // Khởi tạo InMemoryDatabase
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new DataContext(options);

        // Khởi tạo AutoMapper
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CourtPriceResponseDto, CourtPrice>().ReverseMap();
        });
        _mapper = configuration.CreateMapper();

        // Khởi tạo Handler
        _handler = new UpdatePricesOfCourt.Handler(_context, _mapper);
    }

    [TearDown]
    public void Teardown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task Handle_CourtNotExists_ReturnsFailure()
    {
        // Arrange
        var command = new UpdatePricesOfCourt.Command
        {
            CourtId = 1,
            CourtPriceResponseDtos = new List<CourtPriceResponseDto>
            {
               new CourtPriceResponseDto { FromTime = new TimeOnly(14, 30),ToTime = new TimeOnly(16, 30) , Price = 100 }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sân không tồn tại");
    }

    [Test]
    public async Task Handle_CourtExists_UpdatesCourtPrices()
    {
        // Arrange
        var court = new Court
        {
            Id = 1,
            CourtName = "Test Court",
            DeleteAt = null,
            CourtPrices = new List<CourtPrice>
            {
                new CourtPrice { Id = 1, Price = 50 },
                new CourtPrice { Id = 2, Price = 60 }
            }
        };
        _context.Courts.Add(court);
        await _context.SaveChangesAsync();

        var command = new UpdatePricesOfCourt.Command
        {
            CourtId = 1,
            CourtPriceResponseDtos = new List<CourtPriceResponseDto>
            {
                new CourtPriceResponseDto { FromTime = new TimeOnly(14, 30),ToTime = new TimeOnly(16, 30) , Price = 100 },
                new CourtPriceResponseDto { FromTime = new TimeOnly(14, 30),ToTime = new TimeOnly(16, 30), Price = 200 }
            }
        };


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var updatedCourt = await _context.Courts.Include(c => c.CourtPrices).FirstAsync(c => c.Id == 1);
        updatedCourt.CourtPrices.Should().HaveCount(2);
        updatedCourt.CourtPrices.Select(cp => cp.Price).Should().Contain(new[] { 100m, 200m });
    }

    [Test]
    public async Task Handle_EmptyCourtPriceResponseDtos_ClearsCourtPrices()
    {
        // Arrange
        var court = new Court
        {
            Id = 1,
            CourtName = "Test Court",
            DeleteAt = null,
            CourtPrices = new List<CourtPrice>
            {
                new CourtPrice { Id = 1, Price = 50 },
                new CourtPrice { Id = 2, Price = 60 }
            }
        };
        _context.Courts.Add(court);
        await _context.SaveChangesAsync();

        var command = new UpdatePricesOfCourt.Command
        {
            CourtId = 1,
            CourtPriceResponseDtos = new List<CourtPriceResponseDto>()
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var updatedCourt = await _context.Courts.Include(c => c.CourtPrices).FirstAsync(c => c.Id == 1);
        updatedCourt.CourtPrices.Should().BeEmpty();
    }
}

}