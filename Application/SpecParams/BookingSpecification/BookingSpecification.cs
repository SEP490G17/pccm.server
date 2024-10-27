using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                x.StatusName.Equals(baseSpecParam.Filter)
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);

        }
    }
}