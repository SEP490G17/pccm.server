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

        [Required]
        public string PaymentMethod { get; set; }    // Phương thức thanh toán (VNPay, Credit Card, etc.)

        [Required]
        public PaymentStatus Status { get; set; }    // Enum for trạng thái thanh toán (Pending, Success, Failed, etc.)

        public string TransactionRef { get; set; }   // Mã giao dịch từ VNPay hoặc cổng thanh toán khác

        public DateTime CreatedAt { get; set; }      // Thời gian giao dịch

        // Mối quan hệ với Booking
        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        // // Mối quan hệ với Order (có thể là optional nếu không phải thanh toán nào cũng gắn với Order)
        // public int? OrderId { get; set; }

        // [ForeignKey("OrderId")]
        // public Order Order { get; set; }

    }
}