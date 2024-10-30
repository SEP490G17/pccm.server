using Application.Core;
using Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Statistics
{
    public class StatisticExpend
    {
        public class Query : IRequest<Result<ExpendDto>>
        {
            public string Month { get; set; }
            public string Year { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ExpendDto>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<ExpendDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Chuyển đổi Month và Year từ chuỗi sang số nguyên
                if (!int.TryParse(request.Month, out int month) || !int.TryParse(request.Year, out int year))
                {
                    return Result<ExpendDto>.Failure("Invalid month or year format.");
                }

                // Tính tổng chi phí sản phẩm
                var totalProductExpenditure = await _context.Products
                    .Where(p => p.CreatedAt.Month == month && p.CreatedAt.Year == year)
                    .SumAsync(p => p.ImportFee * p.Quantity, cancellationToken);

                // Tính tổng chi phí nhân viên
                var totalStaffExpenditure = await _context.StaffDetails
                /*    .Where(sd => sd.HireDate.Month == month && sd.HireDate.Year == year)*/
                    .SumAsync(sd => sd.Salary, cancellationToken);

                var expenditureDto = new ExpendDto
                {
                    TotalProductExpenditure = totalProductExpenditure,
                    TotalStaffExpenditure = totalStaffExpenditure,
                };

                return Result<ExpendDto>.Success(expenditureDto);
            }
        }
    }
}
