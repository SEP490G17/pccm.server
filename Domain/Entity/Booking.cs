using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Domain.Entity
{
    public class Booking : BaseEntity
    {
        [Required]
        [StringLength(12)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = ""; //Set tạm để lưu demo

        [Required]
        public DateTime StartTime { get; set; }  // Thời gian bắt đầu đặt sân

        [Required]
        public DateTime EndTime { get; set; }  // Thời gian kết thúc đặt sân

        public DateTime? UntilTime { get; set; } = null;
        [Required]
        public int Duration { get; set; } = 60; // Thời lượng đặt sân
        public bool IsSuccess { get; set; } = false;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(1000, double.MaxValue, ErrorMessage = "Tổng giá trị tiền thuê sân phải lớn hơn hoặc bằng 1,000 VND.")]
        public decimal TotalPrice { get; set; }  // Tổng giá trị tiền thuê sân

        [Required]
        public BookingStatus Status { get; set; } = 0;// Trạng thái đặt sân

        public virtual Court? Court { get; set; }  // Liên kết với bảng Courts
        public string AppUserId { get; set; }
        
        [ForeignKey("AppUserId")]
        public virtual AppUser? AppUser { get; set; }  // Liên kết với bảng Users
        public virtual StaffDetail Staff { get; set; }
        public string RecurrenceRule { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? AcceptedAt { get; set; } = null;
        [NotMapped]
        public string StatusName => Status.ToString();  // Trả về tên của trạng thái thay vì số

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); // Liên kết với nhiều Order
        public Payment Payment { get; set; }

    }
}