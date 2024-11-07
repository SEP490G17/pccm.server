using Domain.Entity;

namespace Application.SpecParams.BannerSpec
{
    public class BannersCountSpecification : BaseSpecification<Banner>
    {
        public BannersCountSpecification(BaseSpecParam baseSpecParam) :
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

        }
    }
}