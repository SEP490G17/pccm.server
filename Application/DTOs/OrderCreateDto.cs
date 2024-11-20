namespace Application.DTOs
{
    public class OrderForProductCreateDto
    {
        public int ProductId { get; set; }
        public double Quantity { get; set; }
    }

    public class OrderForServiceCreateDto
    {
        public int ServiceId { get; set; }
    }
}