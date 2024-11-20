using Domain.Entity;

namespace Application.SpecParams.ServiceSpecification
{
    public class ServicesSpecification : BaseSpecification<Service>
    {

        public ServicesSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.ServiceName.ToLower().Contains(baseSpecParam.Search)
            // || x.Description.ToLower().Contains(baseSpecParam.Search)
            ))
            && x.DeletedAt == null
            && (baseSpecParam.Filter == null || baseSpecParam.Filter == 0 || x.CourtClusterId.Equals(baseSpecParam.Filter)))


        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            AddOrderByDescending(x => x.Id);
        }
    }
}