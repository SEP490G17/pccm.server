using System;
using System.Globalization;
using Domain.Entity;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingV2Specification : BaseSpecification<Booking>
    {
        public BookingV2Specification(BookingSpecParam baseSpecParam) : base(
           x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
                x.PhoneNumber.ToLower().Contains(baseSpecParam.Search) ||
                x.FullName.ToLower().Contains(baseSpecParam.Search.ToLower())
                )
           &&
           (
                (baseSpecParam.Status == null
                || baseSpecParam.Status < 0
                || (baseSpecParam.Status == 1 && (int)x.Status == 1 && !x.IsSuccess)
                || (baseSpecParam.Status == 4 && (int)x.Status == 1 && x.IsSuccess)
                || (baseSpecParam.Status != 1 && baseSpecParam.Status != 4 && (int)x.Status == (int)baseSpecParam.Status))
            )
            && (baseSpecParam.CourtClusterId == null || x.Court.CourtCluster.Id == baseSpecParam.CourtClusterId)
            && (baseSpecParam.CourtClusterId == null || x.Court.CourtCluster.Id == baseSpecParam.CourtClusterId)
            && (
                (baseSpecParam.fromDate == null && baseSpecParam.toDate == null) ||
                (
                    x.StartTime >= DateTime.ParseExact(baseSpecParam.fromDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).AddHours(-7)
                    && x.EndTime <= DateTime.ParseExact(baseSpecParam.toDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).AddHours(-7)
                )
            )
        )
        {
            AddOrderByDescending(x => x.CreatedAt);
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
        }
    }
}
