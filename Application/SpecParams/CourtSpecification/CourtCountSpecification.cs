using Domain.Entity;

namespace Application.SpecParams.CourtCountSpecification
{
    public class CourtCountSpecification : BaseSpecification<Court>
    {
        public CourtCountSpecification(BaseSpecWithFilterParam baseSpecWithFilterParam) : base(
           x => (string.IsNullOrEmpty(baseSpecWithFilterParam.Search) ||
            (
                x.CourtName.ToLower().Contains(baseSpecWithFilterParam.Search)
            ))
            && (baseSpecWithFilterParam.Filter == null ||
                (int)x.Status == (baseSpecWithFilterParam.Filter)
            )
        )
        {

        }
    }
}