namespace Application.DTOs
{
    public class BookingConflictDto
    {
        public int BookingId { get; set; }
        public int CourtId { get; set; }
        public DateTime FromDate { get; set; }
        public TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }


    }
}