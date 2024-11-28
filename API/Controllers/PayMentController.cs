using Application.DTOs;
using Application.Handler.Payments;
using Application.Interfaces;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class PayMentController : BaseApiController
    {
        private readonly IVnPayService _vnpayService;
        private readonly DataContext _context;

        public PayMentController(IVnPayService vnpayService, DataContext context)
        {
            _vnpayService = vnpayService;
            _context = context;
        }

        [HttpPost("{type}/{id}/process-payment")]
        public async Task<IActionResult> ProcessPayment(PaymentType type, int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ProcessPayment.Command() { BillPayId = id, Type = type }, ct));
        }

        [HttpGet("vnpay-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> VNPayCallback([FromQuery] VnPayCallbackDto callback)
        {
            var isValid = _vnpayService.VerifyVnPaySignature(callback);
            var paymentStatus = isValid && callback.vnp_ResponseCode.Equals("00")
                                ? PaymentStatus.Success
                                : PaymentStatus.Failed;

            var BillPayId = int.Parse(callback.vnp_TxnRef.Split("_")[0]);
            var type = int.Parse(callback.vnp_TxnRef.Split("_")[1]);

            if (type == (int)PaymentType.Booking)
            {
                var vnpayAmount = decimal.Parse(callback.vnp_Amount);
                var booking = await _context.Bookings.Include(b => b.Payment).FirstAsync(b => b.Id == BillPayId);
                if (booking == null)
                {
                    return NotFound("Booking not found.");
                }
                if (booking.TotalPrice < vnpayAmount)
                {
                    var order = await _context.Orders.Include(o => o.Payment).FirstOrDefaultAsync(o => o.BookingId == BillPayId && o.Payment.Status == PaymentStatus.Pending);
                    var totalAmount = order.TotalAmount + booking.TotalPrice;
                    if (vnpayAmount == totalAmount)
                    {
                        order.Payment.Status = PaymentStatus.Success;
                        _context.Update(order);
                    }

                }
                booking.Payment.Status = paymentStatus;
                _context.Update(booking);

            }
            if (type == (int)PaymentType.Order)
            {
                var order = await _context.Orders.Include(o => o.Payment).FirstAsync(o => o.Id == BillPayId);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }
                order.Payment.Status = paymentStatus;
                _context.Update(order);
            }
            await _context.SaveChangesAsync();
            return Ok(paymentStatus == PaymentStatus.Success ? "Payment successful." : "Payment failed.");
        }
    }
}
