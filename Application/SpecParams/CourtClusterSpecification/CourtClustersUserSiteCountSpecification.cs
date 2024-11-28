using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Application.SpecParams.CourtClusterSpecification
{
    public class CourtClustersUserSiteCountSpecification : BaseSpecification<CourtCluster>
    {
          public CourtClustersUserSiteCountSpecification(CourtClusterSpecParam baseSpecParam) : base(
           x => 
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
                x.DeleteAt == null
            )
            &&
            // Điều kiện phức tạp: Kết hợp với Reviews và CourtPrices
            (
                (!baseSpecParam.Rating.HasValue || 
                 x.Reviews.Any(r => r.Rating == baseSpecParam.Rating)) &&
                (!baseSpecParam.MinPrice.HasValue || 
                 x.Courts.Any(c => c.CourtPrices.Any(cp => cp.Price >= baseSpecParam.MinPrice))) &&
                (!baseSpecParam.MaxPrice.HasValue || 
                 x.Courts.Any(c => c.CourtPrices.Any(cp => cp.Price <= baseSpecParam.MaxPrice)))
            )
        )
        {

        }
    }
}