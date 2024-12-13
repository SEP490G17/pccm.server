using API.SocketSignalR;
using Application.DTOs;
using Application.Handler.Notifications;
using Application.Handler.Payments;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class PayMentController : BaseApiController
    {
        private readonly IVnPayService _vnpayService;
        private readonly DataContext _context;
        private readonly BookingRealTimeService _bookingRealTimeService;
        private readonly IMapper _mapper;
        private readonly NotificationService _notificationService;
        private readonly UserManager<AppUser> _userManager;

        public PayMentController(IVnPayService vnpayService, DataContext context, IMapper mapper, NotificationService notificationService, BookingRealTimeService bookingRealTimeService, UserManager<AppUser> userManager)
        {
            _vnpayService = vnpayService;
            _context = context;
            _mapper = mapper;
            _bookingRealTimeService = bookingRealTimeService;
            _notificationService = notificationService;
            _userManager = userManager;
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
            var paymentStatus = callback.vnp_ResponseCode.Equals("00")
                                ? PaymentStatus.Success
                                : PaymentStatus.Failed;

            var BillPayId = int.Parse(callback.vnp_TxnRef.Split("_")[0]);
            var type = int.Parse(callback.vnp_TxnRef.Split("_")[1]);
            var userName = callback.vnp_TxnRef.Split("_")[2];
            int bookingId = BillPayId;
            if (type == (int)PaymentType.Booking)
            {
                var vnpayAmount = decimal.Parse(callback.vnp_Amount);
                var booking = await _context.Bookings.Include(b => b.Payment).FirstAsync(b => b.Id == BillPayId);
                if (booking == null)
                {
                    return NotFound("Booking not found.");
                }
                if (booking.TotalPrice < vnpayAmount / 100)
                {
                    var order = await _context.Orders.Include(o => o.Payment).FirstOrDefaultAsync(o => o.BookingId == BillPayId && o.Payment.Status == PaymentStatus.Pending);
                    order.Payment.Status = paymentStatus;
                    _context.Update(order);
                }
                booking.Payment.Status = paymentStatus;
                _context.Update(booking);
                var realTimeResponse = await _context.Bookings.ProjectTo<BookingDtoV2>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(b => b.Id == booking.Id);
                if (realTimeResponse != null)
                {
                    realTimeResponse.PaymentStatus = paymentStatus;
                    await _bookingRealTimeService.NotifyUpdateBookingAsync(realTimeResponse, $"admin{realTimeResponse.CourtClusterId}");
                }
                var notiuser = await Mediator.Send(new BookingNotification.Command()
                {
                    BookingId = BillPayId,
                    Message = $"Thánh toán đơn {BillPayId} " + (paymentStatus == PaymentStatus.Success ? "thành công" : "thất bại"),
                    Url = $"{BillPayId}",
                    Title = "Thanh toán",
                    Type = NotificationType.Payment,

                });
                if (notiuser != null)
                {
                    await _notificationService.NotificationForUser(notiuser.NotificationDto, notiuser.PhoneNumber);
                }
            }
            if (type == (int)PaymentType.Order)
            {
                var order = await _context.Orders.Include(o => o.Payment).FirstAsync(o => o.Id == BillPayId);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }
                bookingId = (int)order.BookingId;
                order.Payment.Status = paymentStatus;
                _context.Update(order);
            }
            await _context.SaveChangesAsync();
            var user = await _userManager.FindByNameAsync(userName);
            var roles = await _userManager.GetRolesAsync(user);
            string redirectUrl = $"https://trongnp-registry.site/lich-su/chi-tiet/{bookingId}?"
                + (paymentStatus == PaymentStatus.Success ? "payment=success" : "payment=error");
            if (roles.Any())
            {
                redirectUrl = $"https://admin.trongnp-registry.site/booking/chi-tiet/{bookingId}?"
                + (paymentStatus == PaymentStatus.Success ? "payment=success" : "payment=error");
            }
            return Redirect(redirectUrl);
        }
    }
}
