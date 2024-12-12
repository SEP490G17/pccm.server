using Application.Core;
using Application.DTOs;
using Application.SpecParams;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Handler.News
{
    public class GetTags
    {
        public class Query : IRequest<Result<Pagination<TagsDto>>>
        {
            public BaseSpecParam BaseSpecParam { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Pagination<TagsDto>>>
        {
            private readonly DataContext _context;
            private readonly IMediator _mediator;

            public Handler(DataContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }

            public async Task<Result<Pagination<TagsDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var specParam = request.BaseSpecParam;

                // Lấy 5 thẻ phổ biến nhất
                var getTopTags = await _mediator.Send(new GetMostCommonTags.Query(), cancellationToken);
                List<string> topTags = getTopTags.Value.Select(kvp => kvp.Key).ToList();

                // Truy vấn các NewsBlog mà không thực hiện phần chuyển đổi mảng tại cơ sở dữ liệu
                var newsBlogs = await _context.NewsBlogs
                    .AsNoTracking() // Tối ưu hóa để không theo dõi các thực thể
                    .ToListAsync(cancellationToken);

                // Chuyển đổi các thẻ thành danh sách sau khi đã truy vấn xong
                var tags = newsBlogs
                    .SelectMany(nb => nb.Tags)
                    .Where(tag => !topTags.Contains(tag)) // Loại bỏ các top tags
                    .ToList();

                // Lọc theo điều kiện tìm kiếm và loại bỏ các thẻ trùng lặp
                var filteredTags = tags
                    .Where(tag => string.IsNullOrEmpty(specParam.Search) || tag.Contains(specParam.Search))
                    .Distinct()
                    .Select(tag => new TagsDto { Tag = tag })
                    .ToList();

                // Phân trang kết quả
                var pagedTags = filteredTags
                    .Skip(specParam.Skip)
                    .Take(specParam.PageSize)
                    .ToList();

                var count = filteredTags.Count;

                return Result<Pagination<TagsDto>>.Success(new Pagination<TagsDto>()
                {
                    Count = count,
                    Data = pagedTags,
                    PageSize = specParam.PageSize,
                });
            }
        }
    }
}
