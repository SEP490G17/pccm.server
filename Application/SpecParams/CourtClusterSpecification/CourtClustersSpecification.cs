using Domain.Entity;

namespace Application.SpecParams.CourtClusterSpecification
{
    public class CourtClustersSpecification : BaseSpecification<CourtCluster>
    {
        public CourtClustersSpecification(BaseSpecWithFilterParam baseSpecParam, List<int>? courtClusterAssigns = null) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.CourtClusterName.ToLower().Contains(baseSpecParam.Search) ||
                x.Address.ToLower().Contains(baseSpecParam.Search) ||
                x.WardName.ToLower().Contains(baseSpecParam.Search) ||
                x.DistrictName.ToLower().Contains(baseSpecParam.Search) ||
                x.ProvinceName.ToLower().Contains(baseSpecParam.Search)
            )
            )
            && (baseSpecParam.Filter == null ||
                x.Province.Equals(baseSpecParam.Filter.ToString())
            )
            && x.DeleteAt == null
            && (courtClusterAssigns == null || courtClusterAssigns.Contains(x.Id))

        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            AddOrderByDescending(x => x.CreatedAt);

        }
    }
}