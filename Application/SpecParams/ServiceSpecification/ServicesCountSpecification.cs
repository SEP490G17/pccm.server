using Domain.Entity;

namespace Application.SpecParams.ServiceSpecification
{
    public class ServicesCountSpecification : BaseSpecification<Service>
    {
        public ServicesCountSpecification(BaseSpecWithFilterParam baseSpecParam, List<int> courtClusterId = null) : base(
           x => (string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.ServiceName.ToLower().Contains(baseSpecParam.Search)
            // || x.Description.ToLower().Contains(baseSpecParam.Search)
            ))
            && x.DeletedAt == null
            && (baseSpecParam.Filter == null || baseSpecParam.Filter == 0 || x.CourtClusterId.Equals(baseSpecParam.Filter))
            && (courtClusterId == null || courtClusterId.Contains(x.CourtClusterId.GetValueOrDefault(0)))
            )

        {

        }
    }
}