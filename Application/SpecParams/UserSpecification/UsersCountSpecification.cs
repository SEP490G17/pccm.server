using Domain;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
