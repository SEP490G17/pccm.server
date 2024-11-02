using Domain.Entity;

namespace Application.SpecParams.EventSpecification
{
    public class EventsCountSpecification : BaseSpecification<NewsBlog>
    {
        public EventsCountSpecification(BaseSpecParam baseSpecParam) : base(
            x => string.IsNullOrEmpty(baseSpecParam.Search) ||
                (
                    x.Title.ToLower().Contains(baseSpecParam.Search)
                    // || x.Description.ToLower().Contains(baseSpecParam.Search)
                )
         )
        {

        }
    }
}