using Domain.Entity;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingV2CourtSpecification : BaseSpecification<Booking>
    {
        public BookingV2CourtSpecification(BookingSpecParam baseSpecParam) : base(
           x => (x.Status == null || (int)x.Status == (int)baseSpecParam.Status)
         && (baseSpecParam.CourtClusterId == null || x.Court.CourtCluster.Id == baseSpecParam.CourtClusterId))
        {
            
        }
    }
}