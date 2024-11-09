using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Handler.Payments;
using Application.Interfaces;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Controllers
{
    public class PayMentController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IVnPayService _vnpayService;
        private readonly DataContext _context;

        public PayMentController(IMediator mediator, IVnPayService vnpayService, DataContext context)
        {
            _mediator = mediator;
            _vnpayService = vnpayService;
            _context = context;
        }

        [HttpPost("{id}/process-payment")]
        public async Task<IActionResult> ProcessPayment(int id, decimal amount, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ProcessPayment.Command() { BookingId = id, Amount = amount }, ct));
        }

        [HttpPost("vnpay-callback")]
        public async Task<IActionResult> VNPayCallback([FromQuery] VnPayCallbackDto callback)
        {
            var isValid = _vnpayService.VerifyVnPaySignature(callback);
            var paymentStatus = isValid && callback.vnp_ResponseCode == "00"
                                ? PaymentStatus.Success
                                : PaymentStatus.Failed;

            // Lấy BookingId từ vnp_TxnRef (gửi từ VNPay)
            var bookingId = int.Parse(callback.vnp_TxnRef);
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Tạo bản ghi thanh toán mới
            var payment = new Payment
            {
                BookingId = bookingId,
                Amount = decimal.Parse(callback.vnp_Amount) / 100, // VNPay trả về số tiền với đơn vị cents
                PaymentMethod = "VNPay",
                Status = paymentStatus,
                TransactionRef = callback.vnp_TransactionNo,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            // Cập nhật trạng thái thanh toán của booking nếu thanh toán thành công
            if (paymentStatus == PaymentStatus.Success)
            {
                booking.PaymentStatus = PaymentStatus.Paid;
                _context.Bookings.Update(booking);
            }

            await _context.SaveChangesAsync();
            return Ok(paymentStatus == PaymentStatus.Success ? "Payment successful." : "Payment failed.");
        }
    }
}
