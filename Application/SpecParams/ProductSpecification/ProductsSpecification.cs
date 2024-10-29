using Domain.Entity;

namespace Application.SpecParams.ProductSpecification
{
    public class ProductsSpecification : BaseSpecification<Product>
    {
        public ProductsSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.ProductName.ToLower().Contains(baseSpecParam.Search)
                || x.Description.ToLower().Contains(baseSpecParam.Search)
            ))
            && (baseSpecParam.Filter == null ||
                x.CategoryId.Equals(baseSpecParam.Filter)
            )
            && x.DeletedAt == null
            && x.UpdatedAt == null
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            AddOrderByDescending(x => x.Id);
        }
    }
}