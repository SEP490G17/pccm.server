namespace Application.DTOs
{
    public class OrderForProductCreateDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class OrderForServiceCreateDto
    {
        public int ServiceId { get; set; }
    }
}