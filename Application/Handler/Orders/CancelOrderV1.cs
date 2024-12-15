using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Orders
{
    public class CancelOrderV1
    {
        public class Command : IRequest<Result<OrderOfBookingDto>>
        {
            public int Id { get; set; }

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
                var order = await _context.Orders.Include(o => o.Payment).Include(o=>o.OrderDetails).ThenInclude(od=>od.Product).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (order == null)
                {
                    return Result<OrderOfBookingDto>.Failure("Không tìm thấy Order");
                }
                if (order.Payment.Status == PaymentStatus.Success)
                {
                    return Result<OrderOfBookingDto>.Failure("Order đã được thanh toán không thể cancel");
                }
                if(order.Payment.Status == PaymentStatus.Cancel){
                    return Result<OrderOfBookingDto>.Failure("Order đã được huỷ trước đó");
                }
                var productsRollBack = new List<Product>();
                order.OrderDetails.ToList().ForEach(x=>{
                    if(x.Product != null){
                        var product = x.Product;
                        product.Quantity +=  (decimal)x.Quantity;
                        productsRollBack.Add(product);
                    }
                });
                order.Payment.Status = PaymentStatus.Cancel;
                _context.UpdateRange(productsRollBack);
                _context.Update(order);
                 await _context.SaveChangesAsync(cancellationToken);

                //if (!result) return Result<OrderOfBookingDto>.Failure("Cancel Order thất bại");

                var res = await _context.Orders.ProjectTo<OrderOfBookingDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(o => o.Id == request.Id);
                return Result<OrderOfBookingDto>.Success(res);

            }
        }
    }
}