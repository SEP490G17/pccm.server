using Domain.Entity;

namespace Application.SpecParams.NewsSpecification
{
    public class NewsSpecification : BaseSpecification<NewsBlog>
    {
        public NewsSpecification(NewsSpecParams specParam) : base(
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
            ApplyPaging(specParam.Skip, specParam.PageSize);
            AddOrderByDescending(x => x.CreatedAt);
        }

    }
}
