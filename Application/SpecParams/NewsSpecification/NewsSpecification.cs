using Domain.Entity;

namespace Application.SpecParams.NewsSpecification
{
    public class NewsSpecification : BaseSpecification<NewsBlog>
    {
        public NewsSpecification(BaseSpecParam specParam, List<string> Tags) : base(

            x => (
                (string.IsNullOrEmpty(specParam.Search) ||
                x.Title.Contains(specParam.Search) ||
                x.Description.Contains(specParam.Search)) &&
                (Tags == null || Tags.Count == 0 ||
                x.Tags.AsEnumerable().Any(tag => Tags.Contains(tag)))  // Buộc tính toán phía client
            )
        )
        {
            ApplyPaging(specParam.Skip, specParam.PageSize);
            AddOrderByDescending(x => x.CreatedAt);
        }


    }
}
