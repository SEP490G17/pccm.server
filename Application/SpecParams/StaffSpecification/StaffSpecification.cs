using Domain.Entity;

namespace Application.SpecParams.StaffSpecification
{
    public class StaffSpecification : BaseSpecification<StaffDetail>
    {
        public StaffSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                (x.User.FirstName.Trim() + " " + x.User.LastName.Trim()).ToLower().Contains(baseSpecParam.Search.ToLower())
                ||
                x.User.PhoneNumber.ToLower().Contains(baseSpecParam.Search))
            )

            && (baseSpecParam.Filter == null || baseSpecParam.Filter == -1 ||
                x.Position.Id.Equals(baseSpecParam.Filter + 1)
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            AddOrderByDescending(x => x.User.JoiningDate);
        }
    }
}