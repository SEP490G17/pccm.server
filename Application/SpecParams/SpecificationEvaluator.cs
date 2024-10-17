using Application.SpecParams;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    // Đánh giá các đặc tả
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            // nếu câu lệnh where không null  => add lệnh where
            var query = inputQuery;

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // nếu lệnh orderby không null => add lệnh order by
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            // nếu lệnh orderbyDesc không null => add lệnh order by Desc
            if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            // nếu lệnh paging bất => áp dụng các paging

            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            // Add các lệnh join
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}