using Domain.Entity;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingV2Specification : BaseSpecification<Booking>
    {
        public BookingV2Specification(BookingSpecParam baseSpecParam) : base(
           x => (baseSpecParam.Status == null || (int)x.Status == (int)baseSpecParam.Status)
         && x.Court.CourtCluster.Id == baseSpecParam.CourtClusterId)
        {
            AddOrderByDescending(x => x.CreatedAt);
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
        }

    }
}