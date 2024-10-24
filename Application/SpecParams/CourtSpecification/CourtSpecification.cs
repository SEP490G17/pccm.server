using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Application.SpecParams.CourtSpecification
{
    public class CourtSpecification : BaseSpecification<Court>
    {
        public CourtSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.CourtName.ToLower().Contains(baseSpecParam.Search)
            ))
            && (baseSpecParam.Filter == null ||
                x.Status.Equals(baseSpecParam.Filter)
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);

        }
    }
}