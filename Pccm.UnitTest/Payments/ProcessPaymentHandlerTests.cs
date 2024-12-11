using Application.Handler.Payments;
using Application.Interfaces;
using Domain.Entity;
using Domain.Enum;
using Moq;
using Persistence;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Pccm.UnitTest.Payments
{
    [TestFixture]
    public class ProcessPaymentHandlerTests
    {
        private DataContext _context;
        private Mock<IVnPayService> _vnpayServiceMock;
        private ProcessPayment.Handler _handler;

        [SetUp]
        public void SetUp()
        {
            // Create a mock of IVnPayService
            _vnpayServiceMock = new Mock<IVnPayService>();
            _vnpayServiceMock.Setup(v => v.GeneratePaymentUrl(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<PaymentType>()))
                             .Returns("http://payment-url.com");

            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<DataContext>()
                            .UseInMemoryDatabase(databaseName: "TestDb")
                            .Options;
            _context = new DataContext(options);

            _handler = new ProcessPayment.Handler(_context, _vnpayServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Xóa dữ liệu sau mỗi test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Handle_BookingExists_ReturnsPaymentUrl()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking
            {
                Id = bookingId,
                TotalPrice = 500,
                Payment = null,
                PhoneNumber = "0929939393"
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var command = new ProcessPayment.Command
            {
                BillPayId = bookingId,
                Type = PaymentType.Booking
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("http://payment-url.com");
            var updatedBooking = await _context.Bookings.FindAsync(bookingId);
            updatedBooking.Payment.Should().NotBeNull();
            updatedBooking.Payment.PaymentUrl.Should().Be("http://payment-url.com");
        }

        [Test]
        public async Task Handle_BookingNotFound_ReturnsFailure()
        {
            // Arrange
            var command = new ProcessPayment.Command
            {
                BillPayId = 999, // Invalid booking ID
                Type = PaymentType.Booking
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Booking not found.");
        }

        [Test]
        public async Task Handle_MultipleOrdersPending_ReturnsFailure()
        {
            // Arrange
            var booking = new Booking { Id = 1, TotalPrice = 500,  PhoneNumber = "0929939393" };
            _context.Bookings.Add(booking);
            var order1 = new Order { Id = 1, TotalAmount = 200, BookingId = 1, Payment = new Payment { Status = PaymentStatus.Pending } };
            var order2 = new Order { Id = 2, TotalAmount = 300, BookingId = 1, Payment = new Payment { Status = PaymentStatus.Pending } };
            _context.Orders.AddRange(order1, order2);
            await _context.SaveChangesAsync();

            var command = new ProcessPayment.Command
            {
                BillPayId = 1,
                Type = PaymentType.Booking
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Đang tồn tại 2 order chưa được thanh toán cùng 1 lúc, vui lòng thanh toán trước 1 cái");
        }

        [Test]
        public async Task Handle_OrderExists_ReturnsPaymentUrl()
        {
            // Arrange
            var orderId = 1;
            var order = new Order
            {
                Id = orderId,
                TotalAmount = 1000,
                Payment = null
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var command = new ProcessPayment.Command
            {
                BillPayId = orderId,
                Type = PaymentType.Order
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("http://payment-url.com");
            var updatedOrder = await _context.Orders.FindAsync(orderId);
            updatedOrder.Payment.Should().NotBeNull();
            updatedOrder.Payment.PaymentUrl.Should().Be("http://payment-url.com");
        }

        [Test]
        public async Task Handle_OrderNotFound_ReturnsFailure()
        {
            // Arrange
            var command = new ProcessPayment.Command
            {
                BillPayId = 999, // Invalid order ID
                Type = PaymentType.Order
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Order not found.");
        }

        [Test]
        public async Task Handle_InvalidPaymentType_ReturnsFailure()
        {
            // Arrange
            var command = new ProcessPayment.Command
            {
                BillPayId = 1,
                Type = (PaymentType)999 // Invalid payment type
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Không đúng định dạng");
        }
    }
}
