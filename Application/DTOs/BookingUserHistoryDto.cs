using Domain.Enum;

namespace Application.DTOs
{
    public class BookingUserHistoryDto
    {
        public int Id { get; set; }
        public int CourtClusterId { get; set; }
        public string CourtClusterName { get; set; }
        public string CourtName { get; set; }
        public string Address { get; set; }
        public string TimePlay { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public bool IsSuccess { get; set; } = false;
        public PaymentStatus? PaymentStatus { get; set; }
    }
}