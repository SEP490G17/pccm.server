using Domain.Entity;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingSpecification : BaseSpecification<Booking>
    {
        public BookingSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.AppUser.UserName.ToLower().Contains(baseSpecParam.Search)
            ))
            && (baseSpecParam.Filter == null ||
                (int)x.Status == baseSpecParam.Filter
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            AddOrderByDescending(x=>x.CreatedAt);
        }
    }
}