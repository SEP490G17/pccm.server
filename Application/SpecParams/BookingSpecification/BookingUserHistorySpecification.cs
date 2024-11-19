

using Domain.Entity;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingUserHistorySpecification: BaseSpecification<Booking>
    {
        public BookingUserHistorySpecification(BookingUserHistorySpecParam baseSpecParams):base(
            x=>x.AppUser.Id.Equals(baseSpecParams.UserId)
            &&(baseSpecParams.BookingStatus == null || (int)x.Status == (int)baseSpecParams.BookingStatus)
            &&(baseSpecParams.PaymentStatus == null ||(x.Payment != null && (int)x.Payment.Status == (int)x.Payment.Status))
        )
        {
            ApplyPaging(baseSpecParams.Skip, baseSpecParams.PageSize);
            AddOrderByDescending(x=>x.CreatedAt);
        }
    }
}