using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.DTOs
{
    public class OrderInputDto
    {
         [Key]
        public int Id { get; set; }
        public int? BookingId { get; set; }
        public int? StaffId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;  // Thời gian đặt hàng
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }  // Tổng số tiền của đơn hàng
        [Required]
        public string Status { get; set; }  // Trạng thái đơn hàng: Đang chờ, Đã hoàn thành, hoặc Đã hủy
        [Required]
        public DateTime StartTime { get; set; }  // Thời gian bắt đầu sử dụng sân
        [Required]
        public DateTime EndTime { get; set; }  // Thời gian kết thúc sử dụng sân

    }
}