using Application.Core;
using MediatR;
using Application.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.SpecParams;
using MySqlConnector;

namespace Application.Handler.News
{
    public class ListNewsUserSite
    {
        public class Query : IRequest<Result<Pagination<NewsBlogDto>>>
        {
            public BaseSpecParam NewsSpecParams { get; set; }
            public List<string> Tags { get; set; } = null;
        }

        public class Handler : IRequestHandler<Query, Result<Pagination<NewsBlogDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<Pagination<NewsBlogDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tagsCondition = request.Tags != null && request.Tags.Any()
                    ? "AND " + string.Join(" AND ", request.Tags.Select(tag =>
                    $"JSON_SEARCH(Tags, 'all', '{tag}') IS NOT NULL")) : string.Empty;

                var searchCondition = string.IsNullOrEmpty(request.NewsSpecParams.Search)
                    ? string.Empty
                    : $"AND (Title LIKE @Search OR Description LIKE @Search)";

                var sqlQuery = $@"
                    SELECT *
                    FROM News
                    WHERE 1=1
                    {tagsCondition}
                    {searchCondition}
                    ORDER BY CreatedAt DESC
                    LIMIT @Skip, @PageSize";

                var countQuery = $@"
                    SELECT COUNT(*)
                    FROM News
                    WHERE 1=1
                    {tagsCondition}
                    {searchCondition}";

                var news = await _context.NewsBlogs
                    .FromSqlRaw(sqlQuery,
                        new MySqlParameter("@Search", $"%{request.NewsSpecParams.Search}%"),
                        new MySqlParameter("@Skip", request.NewsSpecParams.Skip),
                        new MySqlParameter("@PageSize", request.NewsSpecParams.PageSize))
                    .ToListAsync(cancellationToken);

                int totalCount;
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = countQuery;
                        command.Parameters.Add(new MySqlParameter("@Search", $"%{request.NewsSpecParams.Search}%"));
                        totalCount = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
                    }
                }

                // Chuyển đổi dữ liệu sang DTO
                var newsDtos = _mapper.Map<List<NewsBlogDto>>(news);

                return Result<Pagination<NewsBlogDto>>.Success(new Pagination<NewsBlogDto>
                {
                    Count = totalCount,
                    Data = newsDtos,
                    PageSize = request.NewsSpecParams.PageSize,
                });
            }
        }
    }
}
