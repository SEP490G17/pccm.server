using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Orders
{
    public class CompleteOrder
    {
        public class Command : IRequest<Result<OrderOfBookingDto>>
        {
            public int Id { get; set; }
        }

        public class Handler(DataContext _context, IMapper _mapper) : IRequestHandler<Command, Result<OrderOfBookingDto>>
        {
            public async Task<Result<OrderOfBookingDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var order = await _context.Orders.FirstAsync(x => x.Id == request.Id, cancellationToken);
                if (order == null) return Result<OrderOfBookingDto>.Failure("Order không tồn tại");
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == request.Id, cancellationToken);
                if (payment == null)
                {
                    var newPayment = new Payment()
                    {
                        Order = order,
                        Amount = order.TotalAmount,
                        Status = Domain.Enum.PaymentStatus.Success,
                    };
                    order.Payment = payment;
                }
                else
                {
                    payment.Status = Domain.Enum.PaymentStatus.Success;
                    _context.Update(payment);
                }
                _context.Update(order);
                await _context.SaveChangesAsync(cancellationToken);
                var response = await _context.Orders.ProjectTo<OrderOfBookingDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                return Result<OrderOfBookingDto>.Success(response);
            }
        }

    }
}