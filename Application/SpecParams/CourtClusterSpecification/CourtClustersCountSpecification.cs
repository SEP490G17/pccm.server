using Domain.Entity;

namespace Application.SpecParams.CourtClusterSpecification
{
    public class CourtClustersCountSpecification : BaseSpecification<CourtCluster>
    {
        public CourtClustersCountSpecification(BaseSpecWithFilterParam baseSpecWithFilterParam, List<int>? courtClusterAssigns = null) : base(
            x => (string.IsNullOrEmpty(baseSpecWithFilterParam.Search) ||
            (
                x.CourtClusterName.ToLower().Contains(baseSpecWithFilterParam.Search) ||
                x.Address.ToLower().Contains(baseSpecWithFilterParam.Search) ||
                x.WardName.ToLower().Contains(baseSpecWithFilterParam.Search) ||
                x.DistrictName.ToLower().Contains(baseSpecWithFilterParam.Search) ||
                x.ProvinceName.ToLower().Contains(baseSpecWithFilterParam.Search)
            )
            )
            && (baseSpecWithFilterParam.Filter == null ||
                x.Province.Equals(baseSpecWithFilterParam.Filter.ToString())
            )
            && x.DeleteAt == null
            && (courtClusterAssigns == null || courtClusterAssigns.Contains(x.Id))
        )
        {

        }
    }
}