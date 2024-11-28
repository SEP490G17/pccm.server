using Application.Core;
using Application.Interfaces;
using Domain.Entity;
using Domain.Enum;
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

                if (request.Type == (int)PaymentType.Booking)
                {
                    var booking = await _context.Bookings.Include(b => b.Payment).FirstOrDefaultAsync(b => b.Id == request.BillPayId,cancellationToken);
                    if (booking == null)
                    {
                        return Result<string>.Failure("Booking not found.");
                    }
                    var order = await _context.Orders.Include(o=>o.Payment).Where(o=>o.Payment.Status == PaymentStatus.Pending && o.BookingId == request.BillPayId).ToListAsync(cancellationToken);
                    if(order.Count > 1){
                        return Result<string>.Failure("Đang tồn tại 2 order chưa được thanh toán cùng 1 lúc, vui lòng thanh toán trước 1 cái");
                    }
                    var amountOfOrder = decimal.Zero;
                    if(order.Count == 1){
                        amountOfOrder = order[0].TotalAmount;
                    }
                    var totalAmount = booking.TotalPrice + amountOfOrder;
                    var paymentUrl = _vnpayService.GeneratePaymentUrl(request.BillPayId, totalAmount, request.Type);

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
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result<string>.Success(paymentUrl);

                }
                if ((int)request.Type == (int)PaymentType.Order)
                {
                    var order = await _context.Orders.Include(o => o.Payment).FirstOrDefaultAsync(o => o.Id == request.BillPayId);
                    if (order == null)
                    {
                        return Result<string>.Failure("Order not found.");
                    }
                    var paymentUrl = _vnpayService.GeneratePaymentUrl(request.BillPayId, order.TotalAmount, request.Type);
                    if (order.Payment == null)
                    {
                        order.Payment = new Payment()
                        {
                            Amount = order.TotalAmount,
                            PaymentMethod = PaymentMethod.VNPay,
                            PaymentUrl = paymentUrl,
                            Status = PaymentStatus.Pending,
                        };
                    }
                    else
                    {
                        order.Payment.PaymentUrl = paymentUrl;
                        order.Payment.PaymentMethod = PaymentMethod.VNPay;
                    }
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result<string>.Success(paymentUrl);

                }
                // Generate VNPay payment URL

                return Result<string>.Failure("Không đúng định dạng");


            }
        }
    }
}