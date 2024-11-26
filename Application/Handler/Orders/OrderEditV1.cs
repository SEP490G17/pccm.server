
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
                var orderDetails = await _context.OrderDetails.Include(o => o.Product).Where(o => o.OrderId == request.Id).ToListAsync(cancellationToken);
                var productsRollBack = new List<Product>();
                if (orderDetails.Count > 0)
                {
                    orderDetails.ForEach(o =>
                    {
                        if (o.Product != null)
                        {
                            var productRollBack = o.Product;
                            productRollBack.Quantity += (decimal)o.Quantity;
                            productsRollBack.Add(productRollBack);
                        }
                    });
                    _context.UpdateRange(productsRollBack);
                    _context.RemoveRange(orderDetails);
                    order.OrderDetails.Clear();
                }
                var products = new List<Product>();

                foreach (var productItem in request.OrderForProducts)
                {
                    var newOrderDetails = new OrderDetail();

                    var product = await _context.Products.FirstOrDefaultAsync(p => productItem.ProductId == p.Id && p.DeletedAt == null, cancellationToken);
                    if (product == null)
                    {
                        return Result<OrderOfBookingDto>.Failure("Sản phẩm không tồn tại vui lòng tải lại trang");
                    }
                    else
                    {
                        var rollbackProduct = productsRollBack.FirstOrDefault(p => p.Id == product.Id);
                        if (rollbackProduct != null)
                        {
                            if (rollbackProduct.Quantity < (decimal)productItem.Quantity)
                            {
                                return Result<OrderOfBookingDto>.Failure("Sản phẩm không đủ để giao dịch, vui lòng tải lại trang");
                            }
                        }
                        else
                        {
                            if (product.Quantity < (decimal)productItem.Quantity)
                            {
                                return Result<OrderOfBookingDto>.Failure("Sản phẩm không đủ để giao dịch, vui lòng tải lại trang");
                            }
                        }
                        newOrderDetails.Product = product;
                        newOrderDetails.ProductId = product.Id;
                        newOrderDetails.Price = product.Price;
                        newOrderDetails.Quantity = productItem.Quantity;
                        order.OrderDetails.Add(newOrderDetails);
                        sum += product.Price * (decimal)productItem.Quantity;
                        product.Quantity -= (decimal)productItem.Quantity;
                        products.Add(product);
                    }

                    order.OrderDetails.Add(newOrderDetails);
                }

                foreach (var serviceItem in request.OrderForServices)
                {
                    var newOrderDetails = new OrderDetail();

                    var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == serviceItem.ServiceId && s.DeletedAt == null, cancellationToken);
                    if (service == null)
                    {
                        return Result<OrderOfBookingDto>.Failure("Dịch vụ không tồn tai vui lòng tải lại trang");
                    }
                    else
                    {
                        newOrderDetails.Service = service;
                        newOrderDetails.ServiceId = service.Id;
                        newOrderDetails.Price = service.Price;
                        newOrderDetails.Quantity = (double)booking.Duration / 60;
                        order.OrderDetails.Add(newOrderDetails);
                        sum += service.Price * booking.Duration / 60;
                    }
                    order.OrderDetails.Add(newOrderDetails);
                }

                order.Payment.Amount = sum;
                order.TotalAmount = sum;
                _context.UpdateRange(products);
                _context.Update(order);
                await _context.SaveChangesAsync(cancellationToken);
                var res = await _context.Orders.ProjectTo<OrderOfBookingDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
                return Result<OrderOfBookingDto>.Success(res);

            }
        }
    }
}