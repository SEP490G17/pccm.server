using Domain.Entity;

namespace Application.DTOs
{
    public class TopStatisticDto
    {
        public List<StaffDto> TopStaffs { get; set; }
        public List<ProductDto> TopProducts { get; set; }
    }
}
