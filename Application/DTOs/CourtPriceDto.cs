

namespace Application.DTOs
{
    public class CourtPriceResponseDto
    {
        public TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }
        public decimal Price { get; set; }

    }
}