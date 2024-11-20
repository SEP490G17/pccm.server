using Domain.Entity;

namespace Application.SpecParams.CourtSpecification
{
  public class CourtOfCourtClusterSpecification : BaseSpecification<Court>
  {

    public CourtOfCourtClusterSpecification(BaseSpecWithFilterParam baseSpecWithFilterParam) : base(
      x => x.CourtClusterId == baseSpecWithFilterParam.Filter
      && x.DeleteAt == null
    )
    {

    }
  }
}