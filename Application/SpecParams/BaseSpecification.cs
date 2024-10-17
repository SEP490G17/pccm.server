using System.Linq.Expressions;

namespace Application.SpecParams
{
    /// <summary>
    /// Đặc tả cho câu lệnh query
    public class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification()
        {
        }
        // Contructor
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria; 
        }

        public Expression<Func<T, bool>> Criteria { get; } // Định nghĩa các lệnh liên quan tới Where

        public List<Expression<Func<T, object>>> Includes { get; } =
            new List<Expression<Func<T, object>>>(); // Join các bảng

        public Expression<Func<T, object>> OrderBy { get; private set; } // sắp xếp theo chiều tăng dần

        public Expression<Func<T, object>> OrderByDescending { get; private set; } // sắp xếp theo chiều giảm dần

        public int Take { get; private set; } // lấy bao nhiêu bản ghi khi query trong bảng

        public int Skip { get; private set; } // Bỏ qua bao nhiêu bản ghi khi query trong bảng

        public bool IsPagingEnabled { get; private set; } // có cho phép paging không

        protected void AddInclude(Expression<Func<T, object>> includeExpression) // Add các lệnh Join vào
        {
            Includes.Add(includeExpression);
        }

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression) // Add các lệnh Orderby vào
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression) // Add các lệnh orderByDesc vào
        {
            OrderByDescending = orderByDescExpression;
        }

        protected void ApplyPaging(int skip, int take) // Apply lệnh phân trang
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }
}