using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Application.SpecParams.ServiceSpecification
{
    public class ServicesCountSpecification : BaseSpecification<Service>
    {
        public ServicesCountSpecification(BaseSpecParam baseSpecParam) : base(
            x => string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.ServiceName.ToLower().Contains(baseSpecParam.Search)
                || x.Description.ToLower().Contains(baseSpecParam.Search)
            )
        )
        {

        }
    }
}