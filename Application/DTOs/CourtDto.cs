

using Domain.Enum;

namespace Application.DTOs
{
    public class CourtDto
    {
        public int CourtId { get; set; }
        public string CourtName { get; set; }
    }

    public class CourtOfClusterDto
    {
        public int CourtId { get; set; }
        public string CourtName { get; set; }
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;

        public List<CourtPriceResponseDto> CourtPrices { get; set; } = null;
        public CourtStatus Status { get; set; }
    }

    public class CourtManagerResponse
    {
        public List<CourtOfClusterDto> CourtForTable { get; set; } = [];
        public string CourtClusterName { get; set; }
    }


}