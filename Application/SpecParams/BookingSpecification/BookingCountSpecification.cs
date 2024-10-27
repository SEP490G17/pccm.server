using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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