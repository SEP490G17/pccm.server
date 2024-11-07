using Domain.Entity;

namespace Application.SpecParams.EventSpecification
{
    public class EventsSpecification : BaseSpecification<NewsBlog>
    {
        public EventsSpecification(BaseSpecParam baseSpecParam) : base(
            x => string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.Title.ToLower().Contains(baseSpecParam.Search)
                // || x.Description.ToLower().Contains(baseSpecParam.Search)
            )
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            if (!string.IsNullOrEmpty(baseSpecParam.Sort))
            {
                switch (baseSpecParam.Sort)
                {
                    case "createAt":
                        AddOrderBy(x => x.CreatedAt);
                        break;
                    case "createAtDesc":
                        AddOrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        AddOrderByDescending(x => x.CreatedAt);
                        break;
                }
            }
            else
            {
                AddOrderByDescending(x => x.CreatedAt);
            }
        }
    }
}