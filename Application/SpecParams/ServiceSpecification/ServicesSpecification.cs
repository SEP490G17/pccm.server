using Domain.Entity;

namespace Application.SpecParams.ServiceSpecification
{
    public class ServicesSpecification : BaseSpecification<Service>
    {
        public ServicesSpecification(BaseSpecParam baseSpecParam) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.ServiceName.ToLower().Contains(baseSpecParam.Search)
                || x.Description.ToLower().Contains(baseSpecParam.Search)
            ))
            && x.DeletedAt == null
            && x.UpdatedAt == null
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);

        }
    }
}