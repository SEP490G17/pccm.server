using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.News
{
    public class GetMostCommonTags
    {
        public class Query : IRequest<Result<List<KeyValuePair<string, int>>>> // Trả về danh sách các tag phổ biến nhất
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<KeyValuePair<string, int>>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<KeyValuePair<string, int>>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Dùng LINQ để ánh xạ kết quả SQL raw mà không cần DbSet
                var mostCommonTags = await _context.Database.SqlQuery<TagCountResult>(
                           @$"
                                SELECT Tag, COUNT(*) as Count
                                FROM (
                                    SELECT 
                                        SUBSTRING_INDEX(SUBSTRING_INDEX(Tags, ',', numbers.n), ',', -1) AS Tag
                                    FROM 
                                        News
                                    JOIN 
                                        (SELECT 1 as n UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 
                                        UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8 
                                        UNION ALL SELECT 9 UNION ALL SELECT 10) numbers
                                    ON CHAR_LENGTH(Tags) - CHAR_LENGTH(REPLACE(Tags, ',', '')) >= numbers.n - 1
                                ) AS TagList
                                WHERE Tag != ''
                                GROUP BY Tag
                                ORDER BY Count DESC
                                LIMIT 10
                            "
                ).ToListAsync(cancellationToken);


                var result = Result<List<KeyValuePair<string, int>>>.Success(
                    mostCommonTags.Select(x => new KeyValuePair<string, int>(x.Tag, x.Count)).ToList()
                );

                return result;
            }
        }
    }
}
public class TagCountResult
{
    public string Tag { get; set; }
    public int Count { get; set; }
}