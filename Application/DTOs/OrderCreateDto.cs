namespace Application.DTOs
{
    public class ProductsForOrderCreateDto
    {
        public int ProductId { get; set; }
        public double Quantity { get; set; }
    }

    public class ServicesForOrderCreateDto
    {
        public int ServiceId { get; set; }
    }
}