using Domain.Enum;

namespace Application.DTOs
{
    public class BookingDetailsDto
    {
        public BookingDtoV2ForDetails BookingDetails { get; set; }
        public IReadOnlyList<OrderOfBookingDto> OrdersOfBooking{ get; set; }
    }

    public class OrderOfBookingDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentStatus  PaymentStatus { get; set; }
        public bool IsOpen { get; set; }
        public decimal TotalAmount { get; set; }
    }
}