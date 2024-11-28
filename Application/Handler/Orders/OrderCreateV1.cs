using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Orders
{
    public class OrderCreateV1
    {
        public class Command : IRequest<Result<OrderOfBookingDto>>
        {
            public int BookingId { get; set; }
            public List<OrderForProductCreateDto> OrderForProducts { get; set; }
            public List<OrderForServiceCreateDto> OrderForServices { get; set; }

        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x).SetValidator(new OrderValidatorV1());


            }
        }


        public class Handler(DataContext _context, IMapper _mapper) : IRequestHandler<Command, Result<OrderOfBookingDto>>
        {
            public async Task<Result<OrderOfBookingDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.OrderForProducts.Count == 0 && request.OrderForServices.Count == 0)
                {
                    return Result<OrderOfBookingDto>.Failure("Danh sách sản phẩm không được rỗng");
                }
                var order = new Order();
                decimal sum = 0;
                var booking = _context.Bookings.Include(b => b.Orders).ThenInclude(c => c.Payment).FirstOrDefault(b => b.Id == request.BookingId);
                if (booking == null) return Result<OrderOfBookingDto>.Failure("Booking không tồn tại");
                if (booking.Orders.Any(b => b.Payment.Status == PaymentStatus.Pending))
                {
                    return Result<OrderOfBookingDto>.Failure("Vui lòng thanh toán Order trước đó");
                }
                var products = new List<Product>();
                foreach (var productItem in request.OrderForProducts)
                {
                    var orderDetails = new OrderDetail();

                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productItem.ProductId && p.DeletedAt == null);
                    if (product != null)
                    {
                        if (product.Quantity < (decimal)productItem.Quantity)
                        {
                            return Result<OrderOfBookingDto>.Failure("Sản phẩm không đủ để giao dịch vui lòng load lại trang");
                        }
                        orderDetails.Product = product;
                        orderDetails.ProductId = product.Id;
                        orderDetails.Price = product.Price;
                        orderDetails.Quantity = productItem.Quantity;
                        order.OrderDetails.Add(orderDetails);
                        sum += product.Price * (decimal)productItem.Quantity;
                        product.Quantity -= (decimal)productItem.Quantity;
                        products.Add(product);
                    }
                    else
                    {
                        return Result<OrderOfBookingDto>.Failure("Sản phẩm đã bị xóa vui lòng tải lại trang");
                    }
                    order.OrderDetails.Add(orderDetails);
                }


                foreach (var serviceItem in request.OrderForServices)
                {
                    var orderDetails = new OrderDetail();
                    var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == serviceItem.ServiceId || s.DeletedAt == null, cancellationToken);
                    if (service != null)
                    {
                        var quantity =  Math.Round((double)booking.Duration / 60, 2);
                        orderDetails.Service = service;
                        orderDetails.ServiceId = service.Id;
                        orderDetails.Price = service.Price;
                        orderDetails.Quantity =  quantity;
                        order.OrderDetails.Add(orderDetails);
                        sum += service.Price * (decimal)quantity;
                    }
                    else
                    {
                        return Result<OrderOfBookingDto>.Failure("Dịch vụ đã bị xóa vui lòng tải lại trang");

                    }
                    order.OrderDetails.Add(orderDetails);
                }

                var payment = new Payment()
                {
                    Amount = sum,
                    Status = PaymentStatus.Pending,
                };
                order.BookingId = request.BookingId;
                order.TotalAmount = sum;
                order.Payment = payment;
                _context.UpdateRange(products);
                await _context.AddAsync(order, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<OrderOfBookingDto>.Failure("Fail to create order");
                return Result<OrderOfBookingDto>.Success(_mapper.Map<OrderOfBookingDto>(order));
            }
        }
    }
}