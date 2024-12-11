using Domain.Entity;

namespace Application.SpecParams.NewsSpecification
{
    public class NewsCountSpecification : BaseSpecification<NewsBlog>
    {
        public NewsCountSpecification(NewsSpecParams specParam) : base
        (
          x => (
                    (
                        string.IsNullOrEmpty(specParam.Search) ||
                        x.Title.Contains(specParam.Search) ||
                        x.Description.Contains(specParam.Search)
                    ) &&
                    (
                        specParam.Tags == null ||
                        specParam.Tags.Any(tag => x.Tags.AsEnumerable().Contains(tag))
                    )
                )
        )
        {

        }
    }
}