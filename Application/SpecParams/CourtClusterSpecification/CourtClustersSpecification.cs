using Domain.Entity;

namespace Application.SpecParams.CourtClusterSpecification
{
    public class CourtClustersSpecification : BaseSpecification<CourtCluster>
    {
        public CourtClustersSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.CourtClusterName.ToLower().Contains(baseSpecParam.Search)
                || x.Description.ToLower().Contains(baseSpecParam.Search)
            ))
            && (baseSpecParam.Filter == null ||
                x.Province.Equals(baseSpecParam.Filter.ToString())
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);

        }
    }
}