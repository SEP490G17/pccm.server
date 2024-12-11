using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Handler.Orders;
using FluentAssertions;

namespace Pccm.UnitTest.Orders
{
    [TestFixture]
    public class CancelOrderHandlerTests
    {
        private DataContext _context;
        private IMapper _mapper;
        private CancelOrderV1.Handler _handler;

        [SetUp]
        public void SetUp()
        {
            // Setup AutoMapper
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = new Mapper(mapperConfig);

            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<DataContext>()
                            .UseInMemoryDatabase(databaseName: "TestDb")
                            .Options;
            _context = new DataContext(options);

            _handler = new CancelOrderV1.Handler(_context, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            // Xóa dữ liệu sau mỗi test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Handle_OrderExistsAndNotPaid_CancelsOrderSuccessfully()
        {
            // Arrange
            var orderId = 1;
            var product = new Product { Id = 1, ProductName = "San phẩm 01", Quantity = 10 };

            // Add the product to the context so it is tracked
            _context.Products.Add(product);
            await _context.SaveChangesAsync();  // Ensure the product is saved first

            var order = new Order
            {
                Id = orderId,
                Payment = new Payment { Status = PaymentStatus.Pending },
                OrderDetails = new List<OrderDetail>
        {
            new OrderDetail { Product = product, Quantity = 2 } // Reference the added product
        }
            };

            // Add the order to the context
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();  // Ensure both product and order are saved

            var command = new CancelOrderV1.Command { Id = orderId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var cancelledOrder = await _context.Orders.Include(o => o.Payment).FirstOrDefaultAsync(o => o.Id == orderId);
            cancelledOrder.Payment.Status.Should().Be(PaymentStatus.Cancel);

            var productInDb = await _context.Products.FirstOrDefaultAsync(p => p.Id == 1);
            productInDb.Quantity.Should().Be(12);  // 10 + 2 (the returned quantity)
        }

        [Test]
        public async Task Handle_OrderNotFound_ReturnsFailure()
        {
            // Arrange
            var command = new CancelOrderV1.Command { Id = 999 }; // Invalid order ID

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Không tìm thấy Order");
        }

        [Test]
        public async Task Handle_OrderAlreadyPaid_ReturnsFailure()
        {
            // Arrange
            var orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Payment = new Payment { Status = PaymentStatus.Success }
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var command = new CancelOrderV1.Command { Id = orderId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Order đã được thanh toán không thể cancel");
        }

        [Test]
        public async Task Handle_OrderAlreadyCancelled_ReturnsFailure()
        {
            // Arrange
            var orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Payment = new Payment { Status = PaymentStatus.Cancel }
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var command = new CancelOrderV1.Command { Id = orderId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Order đã được huỷ trước đó");
        }

        [Test]
        public async Task Handle_SuccessfullyReturnsOrderDtoAfterCancel()
        {
            // Arrange
            var orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Payment = new Payment { Status = PaymentStatus.Pending },
                OrderDetails = new List<OrderDetail>
        {
            new OrderDetail {
                Product = new Product
                {
                    Id = 1,
                    ProductName = "San phẩm 01",  // Ensure this is set correctly
                    Quantity = 10
                },
                Quantity = 2
            }
        }
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var command = new CancelOrderV1.Command { Id = orderId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<OrderOfBookingDto>();
            result.Value.Id.Should().Be(orderId);
        }
    }
}