using Domain.Entity;

namespace Application.SpecParams.ServiceSpecification
{
    public class ServicesCountSpecification : BaseSpecification<Service>
    {
        public ServicesCountSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.ServiceName.ToLower().Contains(baseSpecParam.Search)
                || x.Description.ToLower().Contains(baseSpecParam.Search)
            )
            && x.DeletedAt == null
            && (baseSpecParam.Filter == null || x.CourtClusterId.Equals(baseSpecParam.Filter)))

        {

        }
    }
}