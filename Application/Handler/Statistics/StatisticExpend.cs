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
            public int Month { get; set; }
            public int Year { get; set; }
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
                // Tính tổng chi phí sản phẩm
                var totalProductExpenditure = await _context.Products
                    .Where(p => p.CreatedAt.Month == request.Month && p.CreatedAt.Year == request.Year)
                    .SumAsync(p => p.ImportFee * p.Quantity, cancellationToken);

                // Tính tổng chi phí nhân viên
                var totalStaffExpenditure = await _context.StaffDetails
                   /* .Where(sd => sd.HireDate.Month == request.Month && sd.HireDate.Year == request.Year)*/
                    .SumAsync(sd => sd.Salary, cancellationToken);

                var totalExpenditure = totalProductExpenditure + totalStaffExpenditure;

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
