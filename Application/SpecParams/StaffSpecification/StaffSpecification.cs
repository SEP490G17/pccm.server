using Domain.Entity;

namespace Application.SpecParams.StaffSpecification
{
    public class StaffSpecification : BaseSpecification<StaffDetail>
    {
        public StaffSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.User.UserName.ToLower().Contains(baseSpecParam.Search)
            ))
            && (baseSpecParam.Filter == null ||
                x.ShiftId.Equals(baseSpecParam.Filter)
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);

        }
    }
}