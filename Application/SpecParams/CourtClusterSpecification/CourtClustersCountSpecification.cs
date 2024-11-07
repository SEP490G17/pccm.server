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
                x.Province.Equals(baseSpecWithFilterParam.Filter.ToString())
            )
        )
        {

        }
    }
}