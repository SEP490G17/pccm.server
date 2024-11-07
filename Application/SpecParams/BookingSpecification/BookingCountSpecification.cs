using Domain.Entity;

namespace Application.SpecParams.BookingCountSpecification
{
    public class BookingCountSpecification : BaseSpecification<Booking>
    {
         public BookingCountSpecification(BaseSpecWithFilterParam baseSpecWithFilterParam) : base(
           x => (string.IsNullOrEmpty(baseSpecWithFilterParam.Search) ||
            (
                x.AppUser.UserName.ToLower().Contains(baseSpecWithFilterParam.Search)
            ))
            && (baseSpecWithFilterParam.Filter == null ||
                x.StatusName.Equals(baseSpecWithFilterParam.Filter)
            )
        )
        {

        }
    }
}