using System.Globalization;
using Domain.Entity;
using Domain.Enum;

namespace Application.SpecParams.ProductSpecification
{
    public class ProductsLogSpecification : BaseSpecification<ProductLog>
    {
        public ProductsLogSpecification(ProductLogSpecParams specParam) : base(
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
                    specParam.LogType == null ||
                    (int)x.LogType == specParam.LogType
                )
                && (
                    specParam.fromDate == null && specParam.toDate == null ||

                    x.CreatedAt >= DateTime.ParseExact(specParam.fromDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                    && x.CreatedAt <= DateTime.ParseExact(specParam.toDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                )
        )
        {
            ApplyPaging(specParam.Skip, specParam.PageSize);
            AddOrderByDescending(x => x.Id);
            AddInclude(x => x.CourtCluster);
            AddInclude(x => x.Category);
        }
    }
}
