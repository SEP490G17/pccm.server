using Domain.Enum;

namespace Application.DTOs
{
    public class BookingDtoV2
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string CourtName { get; set; }
        public string PlayTime { get; set; } // Thời gian bắt đầu đặt sân
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string RecurrenceRule { get; set; }
        public string? PaymentUrl { get; set; } = null;
        public BookingStatus Status { get; set; }
        public bool IsSuccess { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class BookingDtoV2ForDetails:BookingDtoV2{
        public int CourtId { get; set; }
        public int CourtClusterId { get; set; }
        public string Address { get; set; }
    }
}