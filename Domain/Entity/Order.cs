using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;


namespace Domain.Entity
{
    public class Order : BaseEntity
    {
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Thời gian đặt hàng
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }  // Tổng số tiền của đơn hàng
        public float? Discount { get; set; }
        public bool IsOpen { get; set; } = true;  // Trạng thái đơn hàng: Đang chờ, Đã hoàn thành, hoặc Đã hủy
        public int? BookingId { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual StaffDetail Staff { get; set; }  // Liên kết với bảng StaffDetails
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public Payment Payment { get; set; }

       
    }
}