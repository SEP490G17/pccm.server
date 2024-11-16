namespace Application.DTOs
{
    public class PriceCourtDto
    {
        public string CourtName { get; set; }
        public int CourtId { get; set; }
        public string Time { get; set; } // khoảng giờ
        public decimal Price { get; set; } // khoảng giờ
    }
}