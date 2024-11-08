using Domain.Entity;

namespace Application.SpecParams.ProductSpecification
{
    public class ProductsSpecification : BaseSpecification<Product>
    {
        public ProductsSpecification(ProductSpecParams specParam) : base(
            x =>
                (
                    string.IsNullOrEmpty(specParam.Search) ||
                    (
                        x.ProductName.ToLower().Contains(specParam.Search) ||
                        x.Description.ToLower().Contains(specParam.Search)
                    )
                )
                && (
                    specParam.CourtCluster == null ||
                    x.CourtClusterId.Equals(specParam.CourtCluster)
                )
                && (
                    specParam.Category == null ||
                    x.CategoryId.Equals(specParam.Category)
                )
                && x.DeletedAt == null // ??m b?o ch? l?y s?n ph?m ch?a b? x�a
        )
        {
            ApplyPaging(specParam.Skip, specParam.PageSize);
            AddOrderByDescending(x => x.Id);
        }
    }
}
