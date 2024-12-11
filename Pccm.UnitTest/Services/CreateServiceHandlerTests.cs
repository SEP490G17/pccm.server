
using Application.Handler.Services;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Core;


namespace Pccm.UnitTest.Services
{
    [TestFixture]
    public class CreateServiceHandlerTests
    {
        private DataContext _context;
        private IMapper _mapper;
        private Create.Handler _handler;

        [SetUp]
        public void SetUp()
        {
            // Tạo DbContext với InMemoryDatabase
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(options);

            // Cấu hình AutoMapper nếu cần
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = mapperConfig.CreateMapper();
            // Làm sạch cơ sở dữ liệu trước khi mỗi test chạy
            _context.CourtClusters.RemoveRange(_context.CourtClusters);
            _context.Services.RemoveRange(_context.Services);
            _context.ServiceLogs.RemoveRange(_context.ServiceLogs);
            // Nếu bạn có các bảng khác, bạn có thể làm sạch chúng ở đây

            _context.SaveChangesAsync();

            // Khởi tạo handler
            _handler = new Create.Handler(_context, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Handle_ServiceAlreadyExists_ReturnsFailure()
        {
            // Arrange: Thêm dữ liệu giả vào cơ sở dữ liệu InMemory
            var serviceName = "Tennis Court";
            var courtClusterId = 1;

            // Cung cấp các thuộc tính bắt buộc cho CourtCluster
            _context.CourtClusters.Add(new CourtCluster
            {
                Id = courtClusterId,
                CourtClusterName = "Court Cluster 1",
                Address = "123 Tennis Street",
                District = "District 1",
                DistrictName = "District Name 1",
                Province = "Province 1",
                ProvinceName = "Province Name 1",
                Ward = "Ward 1",
                WardName = "Ward Name 1"
            });

            _context.Services.Add(new Service
            {
                ServiceName = serviceName,
                CourtClusterId = courtClusterId,
                Price = 1000,
                Description = "Tennis Court Description"
            });

            await _context.SaveChangesAsync();

            // Tạo Command
            var command = new Create.Command
            {
                Service = new ServiceInputDto
                {
                    ServiceName = serviceName,
                    CourtClusterId = new int[] { courtClusterId },
                    Price = 1000,
                    Description = "Tennis Court Description"
                },
                userName = "testuser"
            };

            // Act: Gọi handler để tạo dịch vụ
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert: Kiểm tra xem kết quả trả về có thất bại và có thông báo đúng không
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Dịch vụ 'Tennis Court' đã tồn tại ở Court Cluster 1");
        }

        [Test]
        public async Task Handle_ServiceDoesNotExist_ReturnsSuccess()
        {
            // Arrange: Thêm dữ liệu giả vào cơ sở dữ liệu InMemory
            var serviceName = "Tennis Court Mock";
            var courtClusterId = 1;

            // Cung cấp các giá trị bắt buộc cho CourtCluster
            _context.CourtClusters.Add(new CourtCluster
            {
                Id = courtClusterId,
                CourtClusterName = "Court Cluster 1",
                Address = "123 Tennis Street",
                District = "District 1",
                DistrictName = "District Name 1",
                Province = "Province 1",
                ProvinceName = "Province Name 1",
                Ward = "Ward 1",
                WardName = "Ward Name 1",
                IsVisible = true
            });

            _context.Services.Add(new Service
            {
                ServiceName = "Tennis Court",
                CourtClusterId = courtClusterId,
                Price = 1000,
                Description = "Tennis Court Description"
            });

            await _context.SaveChangesAsync();

            // Tạo Command
            var command = new Create.Command
            {
                Service = new ServiceInputDto
                {
                    ServiceName = serviceName,
                    CourtClusterId = new int[] { courtClusterId },
                    Price = 1000,
                    Description = "Tennis Court Description"
                },
                userName = "testuser"
            };

            // Act: Gọi handler để tạo dịch vụ
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert: Kiểm tra xem kết quả trả về có thành công không
            result.IsSuccess.Should().BeTrue();
            result.Value.Count.Should().Be(1);
        }

      
    }
}
