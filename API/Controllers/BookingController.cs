using API.Extensions;
using Application.DTOs;
using Application.Handler.Bookings;
using Application.SpecParams;
using Application.SpecParams.BookingSpecification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BookingController : BaseApiController
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
        [HttpGet("v1")]
        public async Task<IActionResult> GetBookingsV1([FromQuery] BookingV1SpecParam bookingSpecParam, CancellationToken ct)
        {
            if (bookingSpecParam.FromDate == null)
            {
                bookingSpecParam.FromDate = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).ToUniversalTime();
            }
            if (bookingSpecParam.ToDate == null)
            {
                bookingSpecParam.ToDate = DateTime.Now.EndOfWeek(DayOfWeek.Sunday).ToUniversalTime();
            }
            if(bookingSpecParam.ToDate < bookingSpecParam.FromDate){
                return BadRequest("Thời gian tìm kiếm không hợp lệ");
            }
            return HandleResult(await Mediator.Send(new ListV1.Query() { BookingSpecParam = bookingSpecParam }, ct));
        }

        /// <summary>
        ///  Hàm này dùng để lấy và trả về booking theo cụm sân
        /// </summary>
        /// <param name="bookingSpecParam"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("v2")]
        public async Task<IActionResult> GetBookingV2([FromQuery] BookingSpecParam bookingSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListV2.Query() { BookingSpecParam = bookingSpecParam }, ct));
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooking(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [HttpGet("v1/{id}")]
        public async Task<IActionResult> GetBookingDetails(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new BookingDetailsV1.Query() { Id = id }, ct));
        }
        [AllowAnonymous]
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


        [AllowAnonymous]
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
        [AllowAnonymous]
        [HttpPut("completed/{id}")]
        public async Task<IActionResult> CompletedBooking(int id)
        {
            return HandleResult(await Mediator.Send(new CompletedBooking.Command() { Id = id }));
        }

        /// <summary>
        /// Dùng để xác thực chấp nhận lịch booking từ ngừoi dngf 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trả lại đối tượng tương ứng với đối tượng trả về trong list booking V1</returns>
        [AllowAnonymous]
        [HttpPut("accepted/{id}")]
        public async Task<IActionResult> AcceptedBooking(int id)
        {
            return HandleResult(await Mediator.Send(new AcceptedBooking.Command() { Id = id }));
        }
        /// <summary>
        ///  Dùng để huỷ lịch đặt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            return HandleResult(await Mediator.Send(new CancelBooking.Command() { Id = id }));
        }
        /// <summary>
        /// Dùng để từ chối lịch đặt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("deny/{id}")]
        public async Task<IActionResult> DenyBooking(int id)
        {
            return HandleResult(await Mediator.Send(new DenyBooking.Command() { Id = id }));
        }


        [AllowAnonymous]
        [HttpPost("v2")]
        public async Task<IActionResult> CreateBookingForAdmin([FromBody] BookingInputDto bookingInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new StaffCreate.Command() { Booking = bookingInput }, ct));
        }

        [AllowAnonymous]
        [HttpGet("priceCourt")]
        public async Task<IActionResult> GetListPriceCourtByCourtClusterId([FromQuery] int courtClusterId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new GetCourtPrice.Query() { Id = courtClusterId }, ct));
        }

    }
}