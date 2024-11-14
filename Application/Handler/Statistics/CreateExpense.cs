using Application.Core;
using Application.DTOs;
using Application.Handler.Services;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Statistics
{
    public class CreateExpense
    {
        public class Command : IRequest<Result<ExpenseDto>>
        {
            public ExpenseDto expenseDto { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.expenseDto).SetValidator(new ExpenseValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<ExpenseDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<ExpenseDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var expenseDtos = request.expenseDto;
                //mapping từ ExpenseDto sang List<Expense>
                List<Expense> expenses = expenseDtos.expenseInputDto
                    .Select(input => new Expense
                    {
                        Id = input.Id,
                        CourtClusterId = int.Parse(expenseDtos.CourtClusterId),
                        ExpenseAt = expenseDtos.ExpenseAt,
                        ExpenseName = input.ExpenseName,
                        TotalPrice = input.TotalPrice
                    }).ToList();
                //lấy ra các expenses trong tháng và năm gửi về
                List<Expense> expensesExist = await _context.Expenses
                .Where(e => e.ExpenseAt.Month == expenseDtos.ExpenseAt.Month
                && e.ExpenseAt.Year == expenseDtos.ExpenseAt.Year)
                .ToListAsync();


                foreach (var item in expenses)
                {
                    var existingExpense = expensesExist.FirstOrDefault(e => e.Id == item.Id);
                    //check nếu id tồn tại trong db rồi -> chỉ update
                    if (existingExpense != null)
                    {
                        existingExpense.CourtClusterId = item.CourtClusterId;
                        existingExpense.ExpenseAt = item.ExpenseAt;
                        existingExpense.ExpenseName = item.ExpenseName;
                        existingExpense.TotalPrice = item.TotalPrice;

                        _context.Expenses.Update(existingExpense);
                    }
                    //check nếu id chưa tồn tại trong db -> thêm mới
                    else
                    {
                        await _context.Expenses.AddAsync(item, cancellationToken);
                    }
                }

                foreach (var item in expensesExist)
                {
                    //check nếu trong db có id nhưng dữ liệu gủi về ko có -> đã xóa id đấy -> xóa trong db luôn
                    var existingExpense = expenses.Any(e => e.Id == item.Id);
                    if (!existingExpense)
                    {
                        _context.Expenses.Remove(item);
                    }
                }

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<ExpenseDto>.Failure("Fail to create or update expense");

                return Result<ExpenseDto>.Success(expenseDtos);
            }
        }
    }
}