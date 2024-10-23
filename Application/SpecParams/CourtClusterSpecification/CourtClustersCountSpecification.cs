using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Application.SpecParams.CourtClusterSpecification
{
    public class CourtClustersCountSpecification : BaseSpecification<CourtCluster>
    {
        public CourtClustersCountSpecification(BaseSpecWithFilterParam baseSpecWithFilterParam) : base(
           x => (string.IsNullOrEmpty(baseSpecWithFilterParam.Search) ||
            (
                x.CourtClusterName.ToLower().Contains(baseSpecWithFilterParam.Search)
                || x.Description.ToLower().Contains(baseSpecWithFilterParam.Search)
            ))
            && (baseSpecWithFilterParam.Filter == null ||
                x.Location.Equals(baseSpecWithFilterParam.Filter)
            )
        )
        {

        }
    }
}