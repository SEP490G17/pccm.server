using API.SocketSignalR;
using Application.DTOs;
using Application.Handler.Bookings;
using Application.Handler.Notifications;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.BookingSpecification;
using Domain;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BookingController(IUserAccessor _userAccessor, UserManager<AppUser> _userManager, NotificationService _notificationService) : BaseApiController
    {

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));
        }

        /// <summary>
        ///  Hàm này dùng để lấy về lịch booking theo ngày, theo tuần => dùng cho lịch
        /// </summary>
        /// <param name="bookingSpecParam"></param>
        /// <param name="ct"></param>
        /// <returns></returns> <summary>
        [AllowAnonymous]
        [HttpPost("v1")]
        public async Task<IActionResult> GetBookingsV1([FromBody] BookingV1SpecParam bookingSpecParam, CancellationToken ct)
        {

            return HandleResult(await Mediator.Send(new ListV1.Query() { BookingSpecParam = bookingSpecParam }, ct));
        }

        /// <summary>
        ///  Hàm này dùng để lấy và trả về booking theo cụm sân
        /// </summary>
        /// <param name="bookingSpecParam"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("v2")]
        [Authorize(Roles = "Owner, ManagerCourtCluster, ManagerBooking")]
        public async Task<IActionResult> GetBookingV2([FromQuery] BookingSpecParam bookingSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListV2.Query() { BookingSpecParam = bookingSpecParam }, ct));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [HttpGet("v1/{id}")]

        public async Task<IActionResult> GetBookingDetails(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new BookingDetailsV1.Query() { Id = id }, ct));
        }
        [HttpPut("{id}/{status}")]
        public async Task<IActionResult> UpdateStatusBooking(int id, string status)
        {
            return HandleResult(await Mediator.Send(new Edit.Command() { Id = id, Status = status }));
        }

        [AllowAnonymous]
        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] string date, [FromQuery] int courtClusterId)
        {

            return HandleResult(await Mediator.Send(new GetSlot.Query() { Date = date, CourtClusterId = courtClusterId }));
        }


        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingInputDto bookingInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Booking = bookingInput }, ct));
        }
        /// <summary>
        ///  Dùng để xác thực 1 booking đã được hoàn thành, và sẽ bị đóng lại
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trả lại đối tượng tương ứng với đối tượng trả về trong list booking V1</returns>
        [HttpPut("completed/{id}")]
        [Authorize(Roles = "Admin ,Owner, ManagerCourtCluster, ManagerBooking")]
        public async Task<IActionResult> CompletedBooking(int id)
        {
            var result = await Mediator.Send(new CompletedBooking.Command() { Id = id });
            await HandleAdminUpdateBookingRealTime(result);
            return HandleResult(result);
        }

        /// <summary>
        ///  Dùng để xác thực 1 booking đã được thanh toán, và sẽ bị đóng lại
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trả lại đối tượng tương ứng với đối tượng trả về trong list booking V1</returns>
        [HttpPut("payment-success/{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> PaymentSuccess(int id, [FromQuery] bool includeOrder = false)
        {
            var result = await Mediator.Send(new BookingPaymentSuccess.Command() { Id = id, IncludeOrder = includeOrder });
            await HandleAdminUpdateBookingRealTime(result);
            var notification = await Mediator.Send(new BookingNotification.Command()
            {
                BookingId = id,
                Message = "Lịch đặt của bạn đã được thanh toán thành công",
                Title = "Đặt lịch",
                Type = NotificationType.Booking,
                Url = id.ToString(),
            });
            if (notification != null)
            {
                await _notificationService.NotificationForUser(notification.NotificationDto, notification.PhoneNumber);
            }
            return HandleResult(result);
        }

        /// <summary>
        /// Dùng để xác thực chấp nhận lịch booking từ ngừoi dngf 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trả lại đối tượng tương ứng với đối tượng trả về trong list booking V1</returns>
        [HttpPut("accepted/{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> AcceptedBooking(int id)
        {
            var result = await Mediator.Send(new AcceptedBooking.Command() { Id = id });
            await HandleAdminUpdateBookingRealTime(result);
            await HandleUserUpdateBookingRealTime(result);
            var notification = await Mediator.Send(new BookingNotification.Command()
            {
                BookingId = id,
                Message = "Lịch đặt của bạn đã được xác nhận thành công",
                Title = "Đặt lịch",
                Type = NotificationType.Booking,
                Url = id.ToString(),
            });
            if (notification != null)
            {
                await _notificationService.NotificationForUser(notification.NotificationDto, notification.PhoneNumber);
            }
            return HandleResult(result);
        }

        /// <summary>
        ///  Dùng để huỷ lịch đặt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var result = await Mediator.Send(new CancelBooking.Command() { Id = id });
            await HandleAdminUpdateBookingRealTime(result);
            await HandleUserUpdateBookingRealTime(result);
            var notification = await Mediator.Send(new BookingNotification.Command()
            {
                BookingId = id,
                Message = "Lịch đặt của bạn đã bị hủy",
                Title = "Đặt lịch",
                Type = NotificationType.Booking,
                Url = id.ToString(),
            });
            if (notification != null)
            {
                await _notificationService.NotificationForUser(notification.NotificationDto, notification.PhoneNumber);
            }
            return HandleResult(result);
        }
        /// <summary>
        /// Dùng để từ chối lịch đặt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("deny/{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> DenyBooking(int id)
        {
            var result = await Mediator.Send(new DenyBooking.Command() { Id = id });
            await HandleAdminUpdateBookingRealTime(result);
            var notification = await Mediator.Send(new BookingNotification.Command()
            {
                BookingId = id,
                Message = "Lịch đặt của bạn đã bị từ chối",
                Title = "Đặt lịch",
                Type = NotificationType.Booking,
                Url = id.ToString(),
            });
            if (notification != null)
            {
                await _notificationService.NotificationForUser(notification.NotificationDto, notification.PhoneNumber);
            }
            return HandleResult(result);
        }

        /// <summary>
        /// Tạo booking nhưng phải có role là admin hoặc manager booking
        /// </summary>
        /// <param name="bookingInput"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("v2")]
        public async Task<IActionResult> CreateBookingForAdmin([FromBody] BookingInputDto bookingInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new StaffCreate.Command() { Booking = bookingInput }, ct));
        }

        /// <summary>
        /// Lịch sử đặt sân của user
        /// </summary>
        /// <returns></returns>

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoryBookingOfUser([FromQuery] BookingUserHistorySpecParam baseSpecParam)
        {
            return HandleResult(await Mediator.Send(new UserHistory.Query() { BookingSpecParam = baseSpecParam }));
        }

        [HttpGet("priceCourt")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListPriceCourtByCourtClusterId([FromQuery] int courtClusterId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new GetCourtPrice.Query() { Id = courtClusterId }, ct));
        }

        [HttpPost("combo")]

        public async Task<IActionResult> BookingCombo([FromBody] BookingWithComboDto bookingWithComboDto, CancellationToken ct)
        {
            var result = await Mediator.Send(new BookingWithCombo.Command() { Booking = bookingWithComboDto }, ct);
            await HandleAdminCreateBookingRealTime(result);
            await HandlUserCreateBookingRealTime(result);
            if (result.IsSuccess)
            {

                var user = await _userManager.FindByNameAsync(_userAccessor.GetUserName());
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Count == 0)
                {
                    var notification = await Mediator.Send(new CreateNotification.Command()
                    {
                        Title = "Đặt lịch",
                        Message = "Đặt lịch thành công, vui lòng chờ xác nhận lịch",
                        Type = NotificationType.Booking,
                        Url = $"{result.Value.Id}",
                        AppUser = user,
                    });
                    await _notificationService.NotificationForUser(notification, user.PhoneNumber);
                }
            }
            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpPost("bookingConflict")]

        public async Task<IActionResult> GetBookingConflict([FromBody] BookingConflictDto bookingConflictDto, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new GetBookingConflict.Command() { Booking = bookingConflictDto }, ct));
        }

        [AllowAnonymous]
        [HttpPut("denybookingConflict")]

        public async Task<IActionResult> DenyBookingConflict([FromBody] List<int> bookingId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new DenyBookingConflict.Command() { Id = bookingId }, ct));
        }
        [HttpPost("byDay")]
        public async Task<IActionResult> BookingByDay([FromBody] BookingByDayDto bookingByDay, CancellationToken ct)
        {
            var result = await Mediator.Send(new BookingByDay.Command() { Booking = bookingByDay }, ct);
            await HandleAdminCreateBookingRealTime(result);
            await HandlUserCreateBookingRealTime(result);
            if (result.IsSuccess)
            {

                var user = await _userManager.FindByNameAsync(_userAccessor.GetUserName());
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Count == 0)
                {
                    var notification = await Mediator.Send(new CreateNotification.Command()
                    {
                        Title = "Đặt lịch",
                        Message = "Đặt lịch thành công, vui lòng chờ xác nhận lịch",
                        Type = NotificationType.Booking,
                        Url = $"{result.Value.Id}",
                        AppUser = user,
                    },ct);
                    await _notificationService.NotificationForUser(notification, user.PhoneNumber);
                }
            }
            return HandleResult(result);
        }
    }
}