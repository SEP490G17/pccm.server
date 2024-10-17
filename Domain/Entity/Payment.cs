using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Domain.Entity
{
    public class Payment : BaseEntity
    {
        public int BookingId { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }  // Phương thức thanh toán
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal AmountPaid { get; set; }  // Số tiền đã thanh toán
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;  // Thời gian thanh toán

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }  // Liên kết với bảng Bookings
    }
}