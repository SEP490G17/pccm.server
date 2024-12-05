using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;
using Application.Handler.Reviews;
using Application.Handler.Statistics;

namespace Pccm.UnitTest.Reviews
{
    [TestFixture]
    public class CreateExpenseHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateExpenseHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("b6341ccf-1a22-426c-83bd-21f3f63cd83f", "4", "2024-11-11T10:00:00", ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCreateStatistic_WhenValid(
              string UserId,
              string CourtClusterId,
              string ExpenseAt)
        {
            try
            {
                var expenseDto = new ExpenseDto()
                {
                    CourtClusterId = CourtClusterId,
                    ExpenseAt = DateTime.Parse(ExpenseAt),
                    expenseInputDto = new List<ExpenseInputDto>
                    {
                        new ExpenseInputDto
                        {
                            
                            ExpenseName = "Tien điện",
                            TotalPrice = 1000
                        }
                    }
                };
                var result = await Mediator.Send(new CreateExpense.Command() { expenseDto = expenseDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("b6341ccf-1a22-426c-83bd-21f3f63cd83f", "4", null, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateStatisticFail_WhenExpenseAtIsNull(
              string UserId,
              string CourtClusterId,
              string? ExpenseAt)
        {
            try
            {
                var expenseDto = new ExpenseDto()
                {
                    CourtClusterId = CourtClusterId,
                    ExpenseAt = DateTime.Parse(ExpenseAt),
                    expenseInputDto = new List<ExpenseInputDto>
                    {
                        new ExpenseInputDto
                        {
                            
                            ExpenseName = "Tien điện",
                            TotalPrice = 1000
                        }
                    }
                };
                var result = await Mediator.Send(new CreateExpense.Command() { expenseDto = expenseDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase("b6341ccf-1a22-426c-83bd-21f3f63cd83f", null, "2024-11-11T10:00:00", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateStatisticFail_WhenCourtClusterIDNotExist(
              string UserId,
              string? CourtClusterId,
              string ExpenseAt)
        {
            try
            {
                var expenseDto = new ExpenseDto()
                {
                    CourtClusterId = CourtClusterId,
                    ExpenseAt = DateTime.Parse(ExpenseAt),
                    expenseInputDto = new List<ExpenseInputDto>
                    {
                        new ExpenseInputDto
                        {
                            
                            ExpenseName = "Tien điện",
                            TotalPrice = 1000
                        }
                    }
                };
                var result = await Mediator.Send(new CreateExpense.Command() { expenseDto = expenseDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase("b6341ccf-1a22-426c-83bd-21f3f63cd83f", "4", "2024-11-11T10:00:00", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateStatisticFail_WhenExpenseNameIsNull(
              string UserId,
              string CourtClusterId,
              string ExpenseAt)
        {
            try
            {
                var expenseDto = new ExpenseDto()
                {
                    CourtClusterId = CourtClusterId,
                    ExpenseAt = DateTime.Parse(ExpenseAt),
                    expenseInputDto = new List<ExpenseInputDto>
                    {
                        new ExpenseInputDto
                        {
                            
                            TotalPrice = 1000
                        }
                    }
                };
                var result = await Mediator.Send(new CreateExpense.Command() { expenseDto = expenseDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

          [TestCase("b6341ccf-1a22-426c-83bd-21f3f63cd83f", "4", "2024-11-11T10:00:00", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateStatisticFail_WhenTotalPriceIsNull(
              string UserId,
              string CourtClusterId,
              string ExpenseAt)
        {
            try
            {
                var expenseDto = new ExpenseDto()
                {
                    CourtClusterId = CourtClusterId,
                    ExpenseAt = DateTime.Parse(ExpenseAt),
                    expenseInputDto = new List<ExpenseInputDto>
                    {
                        new ExpenseInputDto
                        {
                            
                            ExpenseName = "Tien điện"
                        }
                    }
                };
                var result = await Mediator.Send(new CreateExpense.Command() { expenseDto = expenseDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
