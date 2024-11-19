using Domain.Entity;

namespace Application.SpecParams.EventSpecification
{
    public class EventsCountSpecification : BaseSpecification<NewsBlog>
    {
        public EventsCountSpecification(BaseSpecWithFilterParam baseSpecParam) : base(
            x => string.IsNullOrEmpty(baseSpecParam.Search) ||
                (
                    x.Title.ToLower().Contains(baseSpecParam.Search)
                )
                &&
                baseSpecParam.Filter == null ||
            (
                (int)x.Status == baseSpecParam.Filter
            )
         )
        {

        }
    }
}