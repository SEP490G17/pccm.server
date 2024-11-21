using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
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
                var booking = _context.Bookings.FirstOrDefault(b => b.Id == request.BookingId);
                request.OrderForProducts.ForEach(orderItem =>
                {
                    var orderDetails = new OrderDetail();

                    var product = _context.Products.Find(orderItem.ProductId);
                    if (product != null)
                    {
                        orderDetails.Product = product;
                        orderDetails.ProductId = product.Id;
                        orderDetails.Price = product.Price;
                        orderDetails.Quantity = orderItem.Quantity;
                        order.OrderDetails.Add(orderDetails);
                        sum += product.Price * (decimal)orderItem.Quantity;
                    }

                    order.OrderDetails.Add(orderDetails);
                });

                request.OrderForServices.ForEach(orderItem =>
               {
                   var orderDetails = new OrderDetail();

                   var service = _context.Services.Find(orderItem.ServiceId);
                   if (service != null)
                   {
                       orderDetails.Service = service;
                       orderDetails.ServiceId = service.Id;
                       orderDetails.Price = service.Price;
                       orderDetails.Quantity = (double)booking.Duration / 60;
                       order.OrderDetails.Add(orderDetails);
                       sum += service.Price * (booking.Duration / 60);
                   }
                   order.OrderDetails.Add(orderDetails);
               });

                var payment = new Payment()
                {
                    Amount = sum,
                    Status = PaymentStatus.Pending,
                };
                order.BookingId = request.BookingId;
                order.TotalAmount = sum;
                order.Payment = payment;
                await _context.AddAsync(order,cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<OrderOfBookingDto>.Failure("Fail to create order");
                return Result<OrderOfBookingDto>.Success(_mapper.Map<OrderOfBookingDto>(order));
            }
        }
    }
}