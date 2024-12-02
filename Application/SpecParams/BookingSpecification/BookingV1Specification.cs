using Domain.Entity;
using Domain.Enum;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingV1Specification : BaseSpecification<Booking>
    {

        public BookingV1Specification(DateTime FromDate, DateTime ToDate, BookingStatus Status, int CourtClusterId) : base
        (
            x =>
                (int)x.Status == (int)Status &&
                x.Court.CourtCluster.Id == CourtClusterId &&
                
                (
                    (
                        x.StartTime.Date >= FromDate.Date &&
                        x.EndTime.Date <= ToDate.Date
                    ) ||
                    x.UntilTime.HasValue && x.UntilTime.Value >= FromDate
                )
        )
        {
            AddOrderByDescending(x => x.CreatedAt);
        }


    }
}