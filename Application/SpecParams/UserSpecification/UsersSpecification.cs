using Domain;

namespace Application.SpecParams.UserSpecification
{
    public class UsersSpecification : BaseSpecification<AppUser>
    {
        public UsersSpecification(BaseSpecParam baseSpecParam) :
        base
        (
            x => string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                (
                  x.Email.ToLower().Contains(baseSpecParam.Search)
                  || x.FirstName.ToLower().Contains(baseSpecParam.Search)
                  || x.LastName.ToLower().Contains(baseSpecParam.Search)
                  || x.PhoneNumber.ToLower().Contains(baseSpecParam.Search)
                )
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
/*            if (!string.IsNullOrEmpty(baseSpecParam.Sort))
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
            }*/
        }
    }
}
