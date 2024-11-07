using Domain.Entity;

namespace Application.SpecParams.ProductSpecification
{
    public class ProductsCountSpecification : BaseSpecification<Product>
    {
        public ProductsCountSpecification(ProductSpecParams specParam) : base(
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
                    specParam.Category == null ||
                    x.CategoryId.Equals(specParam.Category)
                )
                && x.DeletedAt == null // ??m b?o ch? t�nh c�c s?n ph?m ch?a b? x�a
        )
        {
        }
    }
}
