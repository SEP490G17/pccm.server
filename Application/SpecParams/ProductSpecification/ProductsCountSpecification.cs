using Domain.Entity;

namespace Application.SpecParams.ProductSpecification
{
    public class ProductsCountSpecification : BaseSpecification<Product>
    {
        public ProductsCountSpecification(BaseSpecWithFilterParam baseSpecWithFilterParam) : base(
           x => (string.IsNullOrEmpty(baseSpecWithFilterParam.Search) ||
            (
                x.ProductName.ToLower().Contains(baseSpecWithFilterParam.Search)
                || x.Description.ToLower().Contains(baseSpecWithFilterParam.Search)
            ))
            && (baseSpecWithFilterParam.Filter == null ||
                x.CategoryId.Equals(baseSpecWithFilterParam.Filter)
            )
        )
        {

        }
    }
}