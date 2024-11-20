
using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Orders
{
    public class OrderEditV1
    {
        public class Command : IRequest<Result<OrderOfBookingDto>>
        {
            public int Id { get; set; }
            public int BookingId { get; set; }
            public List<OrderForProductCreateDto> OrderForProducts { get; set; }
            public List<OrderForServiceCreateDto> OrderForServices { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {

        }
        public class Handler : IRequestHandler<Command, Result<OrderOfBookingDto>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<OrderOfBookingDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.OrderForProducts.Count == 0 && request.OrderForServices.Count == 0)
                {
                    return Result<OrderOfBookingDto>.Failure("Danh sách sản phẩm không được rỗng");
                }
                var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == request.Id);
                if (order == null)
                {
                    return Result<OrderOfBookingDto>.Failure("Không tìm thấy Order");
                }
                decimal sum = 0;
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == order.BookingId);
                var orderDetails = await _context.OrderDetails.Where(o => o.OrderId == request.Id).ToListAsync();
                if (orderDetails.Count > 0)
                {
                    _context.RemoveRange(orderDetails);
                    order.OrderDetails.Clear();
                }
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
                       orderDetails.Quantity = (double) booking.Duration / 60;
                       order.OrderDetails.Add(orderDetails);
                       sum += service.Price * booking.Duration / 60;
                   }
                   order.OrderDetails.Add(orderDetails);
               });
                order.Payment.Amount = sum;
                order.TotalAmount = sum;
                _context.Update(order);
                await _context.SaveChangesAsync(cancellationToken);
                var res = await _context.Orders.ProjectTo<OrderOfBookingDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(o=>o.Id == request.Id);
                return Result<OrderOfBookingDto>.Success(res);

            }
        }
    }
}