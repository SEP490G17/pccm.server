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
            public string Type { get; set; }
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
                if (request.Type.Equals("bookingBill"))
                {

                    var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BillPayId);
                    if (booking == null)
                    {
                        return Result<string>.Failure("Booking not found.");
                    }
                    booking.PaymentStatus = PaymentStatus.Pending;
                    _context.Bookings.Update(booking);
                }
                if (request.Type.Equals("orderBill"))
                {
                    var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.BillPayId);
                    if (order == null)
                    {
                        return Result<string>.Failure("Order not found.");
                    }
                    // order.Status = PaymentStatus.Pending;
                    _context.Orders.Update(order);
                }
                // Generate VNPay payment URL
                var paymentUrl = _vnpayService.GeneratePaymentUrl(request.BillPayId, request.Amount, request.Type);

                await _context.SaveChangesAsync(cancellationToken);

                return Result<string>.Success(paymentUrl);
            }
        }
    }
}