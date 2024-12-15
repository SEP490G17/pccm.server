using System.Globalization;
using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class CompletedBooking
    {
        public class Command : IRequest<Result<BookingDtoV2>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BookingDtoV2>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<BookingDtoV2>> Handle(Command request, CancellationToken cancellationToken)
            {
                var booking = await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Court)
                .ThenInclude(b => b.CourtCluster)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
                var cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                cultureInfo.NumberFormat.CurrencySymbol = "₫";
                cultureInfo.NumberFormat.CurrencyDecimalDigits = 0;
                cultureInfo.NumberFormat.NumberGroupSeparator = ".";
                cultureInfo.NumberFormat.CurrencyGroupSeparator = ".";
                if (booking == null)
                {
                    return Result<BookingDtoV2>.Failure("Booking không được tìm thấy");
                }
                if ((int)booking.Payment.Status != (int)PaymentStatus.Success)
                {
                    return Result<BookingDtoV2>.Failure("Booking chưa được thanh toán");
                }
                var pendingOrder = _context.Orders.Any(o => o.BookingId == booking.Id && (int)o.Payment.Status == (int)PaymentStatus.Pending);
                if (pendingOrder)
                {
                    return Result<BookingDtoV2>.Failure("Còn đơn Order chưa được thanh toán");
                }
                booking.IsSuccess = true;

                _context.Bookings.Update(booking);

                //log
                var orders = await _context.Orders
                .Where(x => x.BookingId == booking.Id)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Service)
                .ToListAsync(cancellationToken);

                List<ProductLog> productLogs = new List<ProductLog>();
                List<ServiceLog> serviceLogs = new List<ServiceLog>();

                foreach (var order in orders)
                {
                    foreach (var item in order.OrderDetails)
                    {
                        string description = "";
                        if (item.Product != null)
                        {
                            var product = await _context.Products.FirstAsync(x => x.Id == item.ProductId, cancellationToken);
                            description = $"đã bán {item.Quantity} {item.Product.ProductName} với giá bán {string.Format(cultureInfo, "{0:C}", item.Product.Price)}";
                            var productLog = _mapper.Map<ProductLog>(product);
                            productLog.Id = 0;
                            productLog.Quantity = (int)item.Quantity;
                            productLog.CreatedAt = DateTime.Now;
                            productLog.Description = description;
                            productLog.Price = product.ImportFee;
                            productLog.LogType = LogType.Order;
                            productLogs.Add(productLog);
                        }
                        if (item.Service != null)
                        {
                            var service = await _context.Services.FirstAsync(x => x.Id == item.ServiceId, cancellationToken);
                            description = $"đã bán {item.Quantity} dịch vụ {item.Service.ServiceName} với giá bán {string.Format(cultureInfo, "{0:C}", item.Service.Price)}";
                            var serviceLog = _mapper.Map<ServiceLog>(service);
                            serviceLog.Id = 0;
                            serviceLog.CreatedAt = DateTime.Now;
                            serviceLog.Description = description;
                            serviceLog.LogType = LogType.Order;
                            serviceLogs.Add(serviceLog);
                        }
                    }
                }

                await _context.ProductLogs.AddRangeAsync(productLogs, cancellationToken);
                await _context.ServiceLogs.AddRangeAsync(serviceLogs, cancellationToken);
                 await _context.SaveChangesAsync();
                //if (!result) return Result<BookingDtoV2>.Failure("Updated failed booking.");
                return Result<BookingDtoV2>.Success(_mapper.Map<BookingDtoV2>(_context.Entry(booking).Entity));
            }
        }
    }
}