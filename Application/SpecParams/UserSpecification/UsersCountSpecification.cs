using Domain;

namespace Application.SpecParams.UserSpecification
{
    public class UsersCountSpecification : BaseSpecification<AppUser>
    {
        public UsersCountSpecification(BaseSpecParam baseSpecParam) :
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

        }
    }
}
