namespace Application.DTOs
{
    public class BookingDtoV1
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public int CourtId { get; set; }
        public string CourtName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string RecurrenceRule { get; set; }
        public string? UntilTime { get; set; }
        public int? PaymentStatus { get; set; }
        public string? PaymentUrl { get; set; }
        public int Status { get; set; }
        public bool IsSuccess { get; set; }
        public decimal TotalPrice { get; set; }
    }
}