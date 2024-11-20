namespace Application.DTOs
{
    public class ClusterStatisticsDto
    {
        public List<OrderProductDetailDto> OrderProductDetails { get; set; }
        public List<BookingDetailDto> BookingDetails { get; set; }
        public List<OrderServiceDetailDto> OrderServiceDetails { get; set; }
        public List<ExpenseDetailDto> ExpenseDetails { get; set; }
    }
    public class    OrderProductDetailDto
    {
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class ExpenseDetailDto
    {
        public int Id {get; set;}
        public string ExpenseName { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderServiceDetailDto
    {
        public string ServiceName { get; set; }
        public double Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class BookingDetailDto
    {
        public string CourtName { get; set; }
        public string HoursBooked { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
