namespace Application.DTOs
{
    public class ClusterStatisticsDto
    {
        public List<OrderDetailDto> OrderDetails { get; set; }
        public List<BookingDetailDto> BookingDetails { get; set; }
    }
    public class OrderDetailDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class BookingDetailDto
    {
        public string CourtName { get; set; }
        public string HoursBooked { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
