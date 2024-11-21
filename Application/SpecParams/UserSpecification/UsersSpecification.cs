using Domain;

namespace Application.SpecParams.UserSpecification
{
    public class UsersSpecification : BaseSpecification<AppUser>
    {
        public UsersSpecification(BaseSpecParam baseSpecParam) :
        base
        (
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                (
                  x.Email.ToLower().Contains(baseSpecParam.Search)
                  || (x.FirstName.Trim() + " " + x.LastName.Trim()).ToLower().Contains(baseSpecParam.Search.ToLower())
                  || x.PhoneNumber.ToLower().Contains(baseSpecParam.Search)
                )
            ))
            && (string.IsNullOrEmpty(baseSpecParam.Sort) || int.Parse(baseSpecParam.Sort) < 0 ||
            (
                x.LockoutEnabled == int.Parse(baseSpecParam.Sort).Equals(0) ? false : true
            ))
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
