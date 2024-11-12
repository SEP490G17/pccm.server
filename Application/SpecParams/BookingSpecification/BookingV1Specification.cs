using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Entity;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingV1Specification : BaseSpecification<Booking>
    {
        public BookingV1Specification(BookingSpecParam baseSpecParam) : base(
             x => ((int)x.Status == (int)baseSpecParam.BookingStatus)
           && x.StartTime >= baseSpecParam.FromDate
           && x.EndTime <= baseSpecParam.ToDate)
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            AddOrderBy(x => x.CreatedAt);
        }

      
    }
}