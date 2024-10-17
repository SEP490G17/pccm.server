using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Application.SpecParams.ServiceSpecification
{
    public class ServicesSpecification : BaseSpecification<Service>
    {
        public ServicesSpecification(BaseSpecParam baseSpecParam) : base(
            x => string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.ServiceName.ToLower().Contains(baseSpecParam.Search)
                || x.Description.ToLower().Contains(baseSpecParam.Search)
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);

        }
    }
}