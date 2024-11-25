using System.Globalization;
using Domain.Entity;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingV2CourtSpecification : BaseSpecification<Booking>
    {
        public BookingV2CourtSpecification(BookingSpecParam baseSpecParam) : base(
           x => (
                (baseSpecParam.Status == null
                || baseSpecParam.Status < 0
                || ((baseSpecParam.Status == 4) && (int)x.Status == 1 && x.IsSuccess)
                || (int)x.Status == (int)baseSpecParam.Status)
            )
         && (baseSpecParam.CourtClusterId == null || x.Court.CourtCluster.Id == baseSpecParam.CourtClusterId)
                         && (
                    baseSpecParam.fromDate == null && baseSpecParam.toDate == null ||

                    x.StartTime >= DateTime.ParseExact(baseSpecParam.fromDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).AddHours(-7)
                    && x.EndTime <= DateTime.ParseExact(baseSpecParam.toDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).AddHours(-7)
                )
         )

        {

        }
    }
}