using Domain.Enum;

namespace Application.DTOs
{
    public class OrderDetailsResponse
    {
        public int Id { get; set; }
        public int BookingId { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public List<OrderForProductCreateDto> OrderForProducts { get; set; }
        public List<OrderForServiceCreateDto> OrderForServices { get; set; }

    }
}