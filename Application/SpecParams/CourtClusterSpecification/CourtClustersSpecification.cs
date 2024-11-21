using Domain.Entity;

namespace Application.SpecParams.CourtClusterSpecification
{
    public class CourtClustersSpecification : BaseSpecification<CourtCluster>
    {
        public CourtClustersSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.CourtClusterName.ToLower().Contains(baseSpecParam.Search) ||
                x.Address.ToLower().Contains(baseSpecParam.Search) ||
                x.WardName.ToLower().Contains(baseSpecParam.Search) ||
                x.DistrictName.ToLower().Contains(baseSpecParam.Search) ||
                x.ProvinceName.ToLower().Contains(baseSpecParam.Search)
            ))
            && (baseSpecParam.Filter == null ||
                x.Province.Equals(baseSpecParam.Filter.ToString())
            )
            && x.DeleteAt == null

        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);

        }
    }
}