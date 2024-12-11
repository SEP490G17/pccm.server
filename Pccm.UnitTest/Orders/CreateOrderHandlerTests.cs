using Application.Handler.Orders;
using Application.DTOs;
using Domain.Entity;
using Domain.Enum;
using Moq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Core;
using Microsoft.AspNetCore.Http;
using Domain;

namespace Pccm.UnitTest.Orders
{
    [TestFixture]
    public class CreateOrderHandlerTests
    {
        private DbContextOptions<DataContext> _dbContextOptions;
        private DataContext _dbContext;
        private IMapper _mapper;
        private Mock<IMapper> _mockMapper;
        private OrderCreateV1.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database for each test
                 .Options;

            _dbContext = new DataContext(_dbContextOptions);
            _mockMapper = new Mock<IMapper>();

            var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new MappingProfile());
                });
            _mapper = configuration.CreateMapper();

            var userClaims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "administrator"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "ManagerCourtCluster")
            };
            var identity = new System.Security.Claims.ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            var user = new AppUser
            {
                Id = "d6341ccf-1a22-426c-83bd-21f3f63cd83f",
                UserName = "adminstrator",
                FirstName = "Alexandros",
                LastName = "Papadopoulos",
                Email = "adminstrator@test.com"
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            _handler = new OrderCreateV1.Handler(
                _dbContext,
                _mockMapper.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup the in-memory database
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Should_Return_Failure_When_No_Products_Or_Services_Provided()
        {
            var command = new OrderCreateV1.Command
            {
                BookingId = 1,
                OrderForProducts = new List<ProductsForOrderCreateDto>(),
                OrderForServices = new List<ServicesForOrderCreateDto>()
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Danh sách sản phẩm không được rỗng", result.Error);
        }

        [Test]
        public async Task Should_Return_Failure_When_Booking_Not_Found()
        {
            var command = new OrderCreateV1.Command
            {
                BookingId = 999,  // Non-existent booking ID
                OrderForProducts = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto { ProductId = 1, Quantity = 1 }
                },
                OrderForServices = new List<ServicesForOrderCreateDto>
                {
                    new ServicesForOrderCreateDto { ServiceId = 1 }
                }
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Booking không tồn tại", result.Error);
        }

        [Test]
        public async Task Should_Return_Failure_When_Order_Not_Payment()
        {

            var options = new DbContextOptionsBuilder<DataContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;
            var context = new DataContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2024-12-05 03:45:00.000"),
                EndTime = DateTime.Parse("2024-12-05 07:45:00.000"),
                UntilTime = DateTime.Parse("2025-12-05 07:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };

            var order = new Order
            {
                Id = 1,
                BookingId = existingBooking.Id,
                Booking = existingBooking,
                TotalAmount = 1000m,
                Discount = 0f,
                IsOpen = true,
                CreatedBy = 1,
                CreatedAt = DateTime.Now
            };

            var payment = new Payment
            {
                Id = 1,
                OrderId = order.Id,
                Order = order,
                Amount = 1000m,
                Status = PaymentStatus.Pending,
                PaymentMethod = PaymentMethod.VNPay,
                CreatedAt = DateTime.Now,
                TransactionRef = "TXN123456"
            };

            order.Payment = payment;

            existingBooking.Orders = new List<Order> { order };

            await context.Orders.AddAsync(order);
            await context.Payments.AddAsync(payment);
            await context.Bookings.AddAsync(existingBooking);

            await context.SaveChangesAsync();

            var command = new OrderCreateV1.Command
            {
                BookingId = 1,
                OrderForProducts = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto { ProductId = 999, Quantity = 1 }
                },
                OrderForServices = new List<ServicesForOrderCreateDto>()
            };

            var handler = new OrderCreateV1.Handler(context, mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Vui lòng thanh toán Order trước đó", result.Error);
        }


        [Test]
        public async Task Should_Return_Failure_When_Product_Not_Found()
        {

            var options = new DbContextOptionsBuilder<DataContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;
            var context = new DataContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2024-12-05 03:45:00.000"),
                EndTime = DateTime.Parse("2024-12-05 07:45:00.000"),
                UntilTime = DateTime.Parse("2025-12-05 07:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };

            var order = new Order
            {
                Id = 1,
                BookingId = existingBooking.Id,
                Booking = existingBooking,
                TotalAmount = 1000m,
                Discount = 0f,
                IsOpen = true,
                CreatedBy = 1,
                CreatedAt = DateTime.Now
            };

            var payment = new Payment
            {
                Id = 1,
                OrderId = order.Id,
                Order = order,
                Amount = 1000m,
                Status = PaymentStatus.Success,
                PaymentMethod = PaymentMethod.VNPay,
                CreatedAt = DateTime.Now,
                TransactionRef = "TXN123456"
            };

            order.Payment = payment;

            existingBooking.Orders = new List<Order> { order };

            await context.Orders.AddAsync(order);
            await context.Payments.AddAsync(payment);
            await context.Bookings.AddAsync(existingBooking);

            await context.SaveChangesAsync();

            var command = new OrderCreateV1.Command
            {
                BookingId = 1,
                OrderForProducts = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto { ProductId = 999, Quantity = 1 }
                },
                OrderForServices = new List<ServicesForOrderCreateDto>()
            };

            var handler = new OrderCreateV1.Handler(context, mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Sản phẩm đã bị xóa vui lòng tải lại trang", result.Error);
        }

        [Test]
        public async Task Should_Return_Failure_When_Insufficient_Product_Quantity()
        {

            var options = new DbContextOptionsBuilder<DataContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;
            var context = new DataContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2024-12-05 03:45:00.000"),
                EndTime = DateTime.Parse("2024-12-05 07:45:00.000"),
                UntilTime = DateTime.Parse("2025-12-05 07:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };

            var order = new Order
            {
                Id = 1,
                BookingId = existingBooking.Id,
                Booking = existingBooking,
                TotalAmount = 1000m,
                Discount = 0f,
                IsOpen = true,
                CreatedBy = 1,
                CreatedAt = DateTime.Now
            };

            var payment = new Payment
            {
                Id = 1,
                OrderId = order.Id,
                Order = order,
                Amount = 1000m,
                Status = PaymentStatus.Success,
                PaymentMethod = PaymentMethod.VNPay,
                CreatedAt = DateTime.Now,
                TransactionRef = "TXN123456"
            };

            var product = new Product
            {
                Id = 1,
                ThumbnailUrl = "https://example.com/images/product1.jpg",
                CourtClusterId = 1,
                CategoryId = 2,
                ProductName = "Bóng Đá Adidas",
                Description = "Bóng đá Adidas size 5 chính hãng",
                Quantity = 0,
                Price = 500000m,
                ImportFee = 100000m,
            };

            order.Payment = payment;

            existingBooking.Orders = new List<Order> { order };

            await context.Orders.AddAsync(order);
            await context.Payments.AddAsync(payment);
            await context.Products.AddAsync(product);
            await context.Bookings.AddAsync(existingBooking);

            await context.SaveChangesAsync();

            var command = new OrderCreateV1.Command
            {
                BookingId = 1,
                OrderForProducts = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto { ProductId = 1, Quantity = 2 }
                },
                OrderForServices = new List<ServicesForOrderCreateDto>()
            };

            var handler = new OrderCreateV1.Handler(context, mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Sản phẩm không đủ để giao dịch vui lòng load lại trang", result.Error);
        }


        [Test]
        public async Task Should_Return_Failure_When_Service_Not_Found()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
              .Options;
            var context = new DataContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2024-12-05 03:45:00.000"),
                EndTime = DateTime.Parse("2024-12-05 07:45:00.000"),
                UntilTime = DateTime.Parse("2025-12-05 07:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };

            var order = new Order
            {
                Id = 1,
                BookingId = existingBooking.Id,
                Booking = existingBooking,
                TotalAmount = 1000m,
                Discount = 0f,
                IsOpen = true,
                CreatedBy = 1,
                CreatedAt = DateTime.Now
            };

            var payment = new Payment
            {
                Id = 1,
                OrderId = order.Id,
                Order = order,
                Amount = 1000m,
                Status = PaymentStatus.Success,
                PaymentMethod = PaymentMethod.VNPay,
                CreatedAt = DateTime.Now,
                TransactionRef = "TXN123456"
            };

            var product = new Product
            {
                Id = 1,
                ThumbnailUrl = "https://example.com/images/product1.jpg",
                CourtClusterId = 1,
                CategoryId = 2,
                ProductName = "Bóng Đá Adidas",
                Description = "Bóng đá Adidas size 5 chính hãng",
                Quantity = 20,
                Price = 500000m,
                ImportFee = 100000m,
            };

            order.Payment = payment;

            existingBooking.Orders = new List<Order> { order };

            await context.Orders.AddAsync(order);
            await context.Payments.AddAsync(payment);
            await context.Products.AddAsync(product);
            await context.Bookings.AddAsync(existingBooking);

            await context.SaveChangesAsync();

            var command = new OrderCreateV1.Command
            {
                BookingId = 1,
                OrderForProducts = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto { ProductId = 1, Quantity = 2 }
                },
                OrderForServices = new List<ServicesForOrderCreateDto>
                {
                    new ServicesForOrderCreateDto { ServiceId = 999 }
                }
            };

            var handler = new OrderCreateV1.Handler(context, mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Dịch vụ đã bị xóa vui lòng tải lại trang", result.Error);
        }

        [Test]
        public async Task Should_Create_Order_Successfully()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
              .Options;
            var context = new DataContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var user = new AppUser { Id = "user-id", UserName = "testuser", FirstName = "hung", LastName = "Thanh" };
            await context.Users.AddAsync(user);

            var court = new Court { Id = 1, CourtName = "Court 1" };
            await context.Courts.AddAsync(court);

            var existingBooking = new Booking
            {
                Id = 1,
                Court = court,
                StartTime = DateTime.Parse("2024-12-05 03:45:00.000"),
                EndTime = DateTime.Parse("2024-12-05 07:45:00.000"),
                UntilTime = DateTime.Parse("2025-12-05 07:45:00.000"),
                Status = BookingStatus.Confirmed,
                PhoneNumber = "04949494994"
            };

            var order = new Order
            {
                Id = 1,
                BookingId = existingBooking.Id,
                Booking = existingBooking,
                TotalAmount = 1000m,
                Discount = 0f,
                IsOpen = true,
                CreatedBy = 1,
                CreatedAt = DateTime.Now
            };

            var payment = new Payment
            {
                Id = 1,
                OrderId = order.Id,
                Order = order,
                Amount = 1000m,
                Status = PaymentStatus.Success,
                PaymentMethod = PaymentMethod.VNPay,
                CreatedAt = DateTime.Now,
                TransactionRef = "TXN123456"
            };

            var product = new Product
            {
                Id = 1,
                ThumbnailUrl = "https://example.com/images/product1.jpg",
                CourtClusterId = 1,
                CategoryId = 2,
                ProductName = "Bóng Đá Adidas",
                Description = "Bóng đá Adidas size 5 chính hãng",
                Quantity = 20,
                Price = 500000m,
                ImportFee = 100000m,
            };

            var courtCluster = new CourtCluster
            {
                Id = 1,
                CourtClusterName = "Cụm Sân A",
                OpenTime = new TimeOnly(6, 0),
                CloseTime = new TimeOnly(22, 0),
                Province = "01",
                ProvinceName = "Hà Nội",
                District = "001",
                DistrictName = "Quận Hoàn Kiếm",
                Ward = "00001",
                WardName = "Phường Tràng Tiền",
                Address = "123 Phố Tràng Tiền, Quận Hoàn Kiếm, Hà Nội",
                OwnerId = "user-id",
                Description = "Cụm sân với đầy đủ tiện nghi, gần trung tâm thành phố.",
                Images = new string[]
                {
                    "https://example.com/image1.jpg",
                    "https://example.com/image2.jpg"
                },
                CreatedAt = DateTime.Now,
                DeleteAt = null,
                DeleteBy = null,
                IsVisible = true,
            };

            var service = new Service
            {
                Id = 1,
                CourtClusterId = 1,
                ServiceName = "Thuê Huấn Luyện Viên",
                Description = "Dịch vụ thuê huấn luyện viên chuyên nghiệp trong 2 giờ",
                Price = 300000m,
                CourtCluster = courtCluster
            };

            order.Payment = payment;

            existingBooking.Orders = new List<Order> { order
           };

            await context.Orders.AddAsync(order);
            await context.Services.AddAsync(service);
            await context.Payments.AddAsync(payment);
            await context.Products.AddAsync(product);
            await context.Bookings.AddAsync(existingBooking);

            await context.SaveChangesAsync();

            var command = new OrderCreateV1.Command
            {
                BookingId = 1,
                OrderForProducts = new List<ProductsForOrderCreateDto>
                {
                    new ProductsForOrderCreateDto { ProductId = 1, Quantity = 2 }
                },
                OrderForServices = new List<ServicesForOrderCreateDto>
                {
                    new ServicesForOrderCreateDto { ServiceId = 1 }
                }
            };

            var handler = new OrderCreateV1.Handler(context, mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
        }


    }
}
