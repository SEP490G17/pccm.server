using Domain.Entity;

namespace Application.SpecParams.ProductSpecification
{
    public class ProductsLogCountSpecification : BaseSpecification<ProductLog>
    {
        public ProductsLogCountSpecification(ProductLogSpecParams specParam, List<int> courtClusterId = null) : base(
            x =>
                (string.IsNullOrEmpty(specParam.Search) ||
                (
                    x.ProductName.ToLower().Contains(specParam.Search) ||
                    x.Description.ToLower().Contains(specParam.Search)
                ))
                && (
                    specParam.CourtCluster == null ||
                    x.CourtClusterId.Equals(specParam.CourtCluster)
                )
                && (
                    specParam.LogType == null ||
                    x.LogType.Equals(specParam.LogType)
                )
                && (courtClusterId == null || courtClusterId.Contains(x.CourtClusterId.GetValueOrDefault(0)))

        )
        {
        }
    }
}
