using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Application.SpecParams.CourtClusterSpecification
{
    public class CourtClustersUserSiteSpecification : BaseSpecification<CourtCluster>
    {
        public CourtClustersUserSiteSpecification(CourtClusterSpecParam baseSpecParam) : base(x =>
            // Điều kiện cơ bản: Tìm kiếm trong bảng CourtCluster
            (
                (string.IsNullOrEmpty(baseSpecParam.Search) ||
                 x.CourtClusterName.ToLower().Contains(baseSpecParam.Search.ToLower())) &&
                (string.IsNullOrEmpty(baseSpecParam.Province) ||
                 x.Province.ToLower().Contains(baseSpecParam.Province.ToLower())) &&
                (string.IsNullOrEmpty(baseSpecParam.District) ||
                 x.District.ToLower().Contains(baseSpecParam.District.ToLower())) &&
                (string.IsNullOrEmpty(baseSpecParam.Ward) ||
                 x.Ward.ToLower().Contains(baseSpecParam.Ward.ToLower())) &&
                (!baseSpecParam.Rating.HasValue ||
                 (baseSpecParam.Rating >= 4 && x.Reviews.Any() && x.Reviews.Average(r => r.Rating) % 1 >= 0.5
                 ?
                 Math.Ceiling(x.Reviews.Average(r => r.Rating)) <= baseSpecParam.Rating
                 && Math.Ceiling(x.Reviews.Average(r => r.Rating)) >= baseSpecParam.Rating - 1
                 :
                 Math.Floor(x.Reviews.Average(r => r.Rating)) <= baseSpecParam.Rating
                 && Math.Floor(x.Reviews.Average(r => r.Rating)) >= baseSpecParam.Rating - 1
                 )
                 ||
                 (baseSpecParam.Rating == 3 && x.Reviews.Any() && x.Reviews.Average(r => r.Rating) % 1 >= 0.5
                 ?
                 Math.Ceiling(x.Reviews.Average(r => r.Rating)) <= baseSpecParam.Rating
                 && Math.Ceiling(x.Reviews.Average(r => r.Rating)) >= 1
                 :
                 Math.Floor(x.Reviews.Average(r => r.Rating)) <= baseSpecParam.Rating
                 && Math.Floor(x.Reviews.Average(r => r.Rating)) >= 1
                 )
                ) &&
                (!baseSpecParam.MinPrice.HasValue && !baseSpecParam.MinPrice.HasValue ||
                 baseSpecParam.MinPrice.HasValue && baseSpecParam.MinPrice.HasValue &&
                 x.Courts.Any(c => c.CourtPrices.Any(cp => cp.Price >= baseSpecParam.MinPrice && cp.Price <= baseSpecParam.MaxPrice))
                )
                && x.DeleteAt == null
            )
       )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            AddOrderByDescending(x => x.Id);
        }
    }
}