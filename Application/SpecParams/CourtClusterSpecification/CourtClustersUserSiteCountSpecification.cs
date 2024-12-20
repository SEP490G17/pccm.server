using Domain.Entity;

namespace Application.SpecParams.CourtClusterSpecification
{
    public class CourtClustersUserSiteCountSpecification : BaseSpecification<CourtCluster>
    {
        public CourtClustersUserSiteCountSpecification(CourtClusterSpecParam baseSpecParam) : base(
         x =>
          // Điều kiện cơ bản: Tìm kiếm trong bảng CourtCluster
          (string.IsNullOrEmpty(baseSpecParam.Search) ||
               x.CourtClusterName.ToLower().Contains(baseSpecParam.Search.ToLower())) &&
              (string.IsNullOrEmpty(baseSpecParam.Province) ||
               x.Province.ToLower().Contains(baseSpecParam.Province.ToLower())) &&
              (string.IsNullOrEmpty(baseSpecParam.District) ||
               x.District.ToLower().Contains(baseSpecParam.District.ToLower())) &&
              (string.IsNullOrEmpty(baseSpecParam.Ward) ||
               x.Ward.ToLower().Contains(baseSpecParam.Ward.ToLower())) &&

              (!baseSpecParam.Rating.HasValue ||
               (x.Reviews.Any()
               &&
               (
                  baseSpecParam.Rating == 5 && Math.Round(x.Reviews.Average(r => r.Rating), 0) <= baseSpecParam.Rating && Math.Round(x.Reviews.Average(r => r.Rating), 0) >= baseSpecParam.Rating - 1
                  ||
                  baseSpecParam.Rating == 3 && Math.Round(x.Reviews.Average(r => r.Rating), 0) <= baseSpecParam.Rating && Math.Round(x.Reviews.Average(r => r.Rating), 0) >= baseSpecParam.Rating - 1
                  ||
                  baseSpecParam.Rating == 1 && Math.Round(x.Reviews.Average(r => r.Rating), 0) == baseSpecParam.Rating
               )))
               &&
              (!baseSpecParam.MinPrice.HasValue ||
               x.Courts.Any(c =>
                   c.CourtPrices.Any(cp => cp.Price >= baseSpecParam.MinPrice && cp.Price <= baseSpecParam.MaxPrice))) &&

              x.DeleteAt == null
      )
        {

        }
    }
}