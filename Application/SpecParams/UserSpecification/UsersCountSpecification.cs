using Domain;

namespace Application.SpecParams.UserSpecification
{
    public class UsersCountSpecification : BaseSpecification<AppUser>
    {
        public UsersCountSpecification(BaseSpecParam baseSpecParam) :
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
                x.IsDisabled == int.Parse(baseSpecParam.Sort).Equals(0) ? false : true
            ))
        )
        {

        }
    }
}
