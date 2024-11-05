using Domain.Entity;

namespace Application.SpecParams.CourtCountSpecification
{
    public class StaffCountSpecification : BaseSpecification<StaffDetail>
    {
        public StaffCountSpecification(BaseSpecWithFilterParam baseSpecWithFilterParam) : base(
           x => (string.IsNullOrEmpty(baseSpecWithFilterParam.Search) ||
            (
                x.User.UserName.ToLower().Contains(baseSpecWithFilterParam.Search)
            ))
            && (baseSpecWithFilterParam.Filter == null ||
                x.ShiftId.Equals(baseSpecWithFilterParam.Filter)
            )
        )
        {

        }
    }
}