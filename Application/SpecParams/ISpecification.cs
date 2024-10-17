using System.Linq.Expressions;

namespace Application.SpecParams
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; } // Các lệnh truy vấn Where
        List<Expression<Func<T, object>>> Includes { get; } // Các lệnh Join
        Expression<Func<T, object>> OrderBy { get; } // Các lệnh sắp xếp tăng dần
        Expression<Func<T, object>> OrderByDescending { get; } // Các lệnh sắp xếp giảm dần
        int Take { get; } // Số lượng bản ghi được lấy trong câu query
        int Skip { get; } // Số lượng bản ghi bị bỏ qua trong câu query
        bool IsPagingEnabled { get; } // Bật tắt chức năng paging
    }
}