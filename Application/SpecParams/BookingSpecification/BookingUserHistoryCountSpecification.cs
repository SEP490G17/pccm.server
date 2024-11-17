using Domain.Entity;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingUserHistoryCountSpecification : BaseSpecification<Booking>
    {
        public BookingUserHistoryCountSpecification(BookingUserHistorySpecParam baseSpecParams) : base(
           x => x.AppUser.Id.Equals(baseSpecParams.UserId)
           && (baseSpecParams.BookingStatus != null || (int)x.Status == (int)baseSpecParams.BookingStatus)
           && (baseSpecParams.PaymentStatus != null || (x.Payment != null && (int)x.Payment.Status == (int)x.Payment.Status))
       )
        {

        }
    }
}