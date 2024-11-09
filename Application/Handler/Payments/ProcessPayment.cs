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
            public int BookingId { get; set; }
            public decimal Amount { get; set; }
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
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId);
                if (booking == null)
                {
                    return Result<string>.Failure("Booking not found.");
                }

                // Generate VNPay payment URL
                var paymentUrl = _vnpayService.GeneratePaymentUrl(booking.Id, request.Amount);
                booking.PaymentStatus = PaymentStatus.Pending;
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<string>.Success(paymentUrl);
            }
        }
    }
}