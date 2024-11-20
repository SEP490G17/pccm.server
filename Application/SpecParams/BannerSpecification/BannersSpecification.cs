using Application.SpecParams.ProductSpecification;
using Domain.Entity;

namespace Application.SpecParams
{
    public class BannersSpecification : BaseSpecification<Banner>
    {
        public BannersSpecification(BannerSpecParams baseSpecParam) :
        base
        (
            x =>
                (x.DeletedAt == null) &&
                (string.IsNullOrEmpty(baseSpecParam.Search) ||
                (
                    x.Title.ToLower().Contains(baseSpecParam.Search)
                ))
                &&
            (
                baseSpecParam.status == null ||
                (baseSpecParam.status >= 0 && (int)x.Status == baseSpecParam.status) ||
                baseSpecParam.status < 0
            )
            &&
            (
                baseSpecParam.category == null ||
                (baseSpecParam.category >= 0 && (int)x.BannerType == baseSpecParam.category) ||
                baseSpecParam.category < 0
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            if (!string.IsNullOrEmpty(baseSpecParam.Sort))
            {
                switch (baseSpecParam.Sort)
                {
                    case "createAt":
                        AddOrderBy(x => x.CreatedAt);
                        break;
                    case "createAtDesc":
                        AddOrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        AddOrderByDescending(x => x.CreatedAt);
                        break;
                }
            }
            else
            {
                AddOrderByDescending(x => x.CreatedAt);
            }
        }
    }
}