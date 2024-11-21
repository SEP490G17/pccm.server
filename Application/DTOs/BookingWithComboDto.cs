namespace Application.DTOs
{
    public class BookingWithComboDto
    {
        public int CourtId { get; set; }
        public int ComboId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime FromDate { get; set; }
        public TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }


    }
}