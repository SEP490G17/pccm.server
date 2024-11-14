namespace Application.DTOs
{
    public class ExpenseDto
    {
        public string CourtClusterId { get; set; }
        public DateTime ExpenseAt { get; set; }
        public List<ExpenseInputDto> expenseInputDto { get; set; }
    }

    public class ExpenseInputDto
    {
        public int Id { get; set; }
        public string ExpenseName { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
