using Domain.Entity;

namespace Application.SpecParams.ServiceSpecification
{
    public class ServicesCountSpecification : BaseSpecification<Service>
    {
        public ServicesCountSpecification(BaseSpecParam baseSpecParam) : base(
            x => string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.ServiceName.ToLower().Contains(baseSpecParam.Search)
                || x.Description.ToLower().Contains(baseSpecParam.Search)
            )
        )
        {

        }
    }
}