using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Domain.Entity
{
    public class Payment : BaseEntity
    {
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }          // Số tiền thanh toán

        public PaymentMethod? PaymentMethod { get; set; } = null;    // Phương thức thanh toán (VNPay, Credit Card, etc.)

        public string? PaymentUrl { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }    // Enum for trạng thái thanh toán (Pending, Success, Failed, etc.)

        public string TransactionRef { get; set; } = string.Empty;  // Mã giao dịch từ VNPay hoặc cổng thanh toán khác

        public DateTime CreatedAt { get; set; } = DateTime.Now;     // Thời gian giao dịch
        public DateTime? PaidAt { get; set; } = null;     // Thời gian giao dịch


        // Mối quan hệ với Booking
        public int? BookingId { get; set; } = null;

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        public int? OrderId { get; set; } = null;

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

    }
}