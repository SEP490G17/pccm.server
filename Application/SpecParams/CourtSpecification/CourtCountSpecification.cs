using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Application.SpecParams.CourtCountSpecification
{
    public class CourtCountSpecification : BaseSpecification<Court>
    {
        public CourtCountSpecification(BaseSpecWithFilterParam baseSpecWithFilterParam) : base(
           x => (string.IsNullOrEmpty(baseSpecWithFilterParam.Search) ||
            (
                x.CourtName.ToLower().Contains(baseSpecWithFilterParam.Search)
            ))
            && (baseSpecWithFilterParam.Filter == null ||
                x.Status.Equals(baseSpecWithFilterParam.Filter)
            )
        )
        {

        }
    }
}