using Domain.Enum;

namespace Application.DTOs
{
    public class OrderDetailsResponse
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<ProductsForOrderDetailsResponse> OrderForProducts { get; set; }
        public List<ServicesForOrderDetailsResponse> OrderForServices { get; set; }

    }

    public class ProductsForOrderDetailsResponse
    {
        public int ProductId { get; set; }
        public double Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal CurrPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal CurrTotalPrice { get; set; }
    }

    public class ServicesForOrderDetailsResponse
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public decimal CurrPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal CurrTotalPrice { get; set; }
    }
}