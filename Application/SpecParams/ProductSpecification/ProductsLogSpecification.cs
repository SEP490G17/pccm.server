using System.Globalization;
using Domain.Entity;

namespace Application.SpecParams.ProductSpecification
{
    public class ProductsLogSpecification : BaseSpecification<ProductLog>
    {
        public ProductsLogSpecification(ProductLogSpecParams specParam, List<int> courtClusterId = null) : base(
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
                && (courtClusterId == null || courtClusterId.Contains(x.CourtClusterId.GetValueOrDefault(0)))
        )
        {
            ApplyPaging(specParam.Skip, specParam.PageSize);
            AddOrderByDescending(x => x.Id);
            AddInclude(x => x.CourtCluster);
            AddInclude(x => x.Category);
        }
    }
}
