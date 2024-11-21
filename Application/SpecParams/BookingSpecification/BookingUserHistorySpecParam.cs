

using Domain.Enum;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingUserHistorySpecParam:BaseSpecParam
    {
        public string? UserId { get; set; }
        public BookingStatus? BookingStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}