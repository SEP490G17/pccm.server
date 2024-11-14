using Domain.Enum;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingSpecParam : BaseSpecParam
    {
        public int CourtClusterId { get; set; }
        public BookingStatus? Status { get; set; } = null;
    }
}