using Domain.Entity;

namespace Application.SpecParams
{
    public class BannersSpecification : BaseSpecification<Banner>
    {
        public BannersSpecification(BaseSpecParam baseSpecParam) :
        base
        (
            x => 
                (x.DeletedAt == null) && 
                (string.IsNullOrEmpty(baseSpecParam.Search) || 
                (
                    x.Title.ToLower().Contains(baseSpecParam.Search) 
                ))
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