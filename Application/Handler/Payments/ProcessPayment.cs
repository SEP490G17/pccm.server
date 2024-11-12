using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Payments
{

    public class ProcessPayment
    {
        public class Command : IRequest<Result<string>>
        {
            public int BillPayId { get; set; }
            public decimal Amount { get; set; }
            public PaymentType Type { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly DataContext _context;
            private readonly IVnPayService _vnpayService;
            public Handler(DataContext context, IVnPayService vnpayService)
            {
                _context = context;
                _vnpayService = vnpayService;
            }

            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var paymentUrl = _vnpayService.GeneratePaymentUrl(request.BillPayId, request.Amount, request.Type);

                if ((int)request.Type == (int)PaymentType.Booking)
                {
                    var booking = await _context.Bookings.Include(b => b.Payment).FirstOrDefaultAsync(b => b.Id == request.BillPayId);
                    if (booking == null)
                    {
                        return Result<string>.Failure("Booking not found.");
                    }
                    if (booking.Payment == null)
                    {
                        booking.Payment = new Payment()
                        {
                            Amount = booking.TotalPrice,
                            PaymentMethod = PaymentMethod.VNPay,
                            PaymentUrl = paymentUrl,
                            Status = PaymentStatus.Pending,
                        };
                    }
                    else
                    {
                        booking.Payment.PaymentUrl = paymentUrl;
                        booking.Payment.PaymentMethod = PaymentMethod.VNPay;
                    }
                    _context.Bookings.Update(booking);
                }
                if ((int)request.Type == (int)PaymentType.Order)
                {
                    var order = await _context.Orders.Include(o => o.Payment).FirstOrDefaultAsync(o => o.Id == request.BillPayId);
                    if (order == null)
                    {
                        return Result<string>.Failure("Order not found.");
                    }
                    if(order.Payment == null){
                        order.Payment = new Payment()
                        {
                            Amount = order.TotalAmount,
                            PaymentMethod = PaymentMethod.VNPay,
                            PaymentUrl = paymentUrl,
                            Status = PaymentStatus.Pending,
                        };
                    }else{
                        order.Payment.PaymentUrl = paymentUrl;
                        order.Payment.PaymentMethod = PaymentMethod.VNPay;
                    }
                    _context.Orders.Update(order);
                }
                // Generate VNPay payment URL

                await _context.SaveChangesAsync(cancellationToken);

                return Result<string>.Success(paymentUrl);
            }
        }
    }
}