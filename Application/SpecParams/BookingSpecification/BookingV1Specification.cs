using Domain.Entity;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingV1Specification : BaseSpecification<Booking>
    {
        public BookingV1Specification(BookingV1SpecParam baseSpecParam) : base(
             x => ((int)x.Status == (int)baseSpecParam.BookingStatus)
           && x.Court.CourtCluster.Id == baseSpecParam.CourtClusterId
           && x.StartTime >= baseSpecParam.FromDate
           && x.EndTime <= baseSpecParam.ToDate)
        {
            AddOrderByDescending(x => x.CreatedAt);
        }

      
    }
}