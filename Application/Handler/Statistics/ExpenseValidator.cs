using Application.DTOs;
using FluentValidation;

namespace Application.Handler.Services
{
    public class ExpenseValidator : AbstractValidator<ExpenseDto>
    {
        public ExpenseValidator()
        {
            RuleFor(x => x.CourtClusterId).NotEmpty().WithMessage("CourtClusterId is required");
            RuleForEach(x => x.expenseInputDto).ChildRules(expense =>
            {
                expense.RuleFor(e => e.ExpenseName).NotEmpty().WithMessage("ExpenseName is required");
                expense.RuleFor(e => e.TotalPrice).NotEmpty().WithMessage("TotalPrice is required");
            });
            RuleFor(x => x.ExpenseAt).NotEmpty().WithMessage("ExpenseAt is required");
        }
    }
}
