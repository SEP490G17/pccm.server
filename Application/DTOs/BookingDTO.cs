using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain;
using Domain.Entity;

namespace Application.DTOs
{
    public class BookingDTO
    {
        [Key]
        public int Id { get; set; }
        public int CourtId { get; set; }

        public string UserId { get; set; }

        public int StaffId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }  // Thời gian bắt đầu đặt sân

        [Required]
        public DateTime EndTime { get; set; }  // Thời gian kết thúc đặt sân

        [Required]
        public int Duration { get; set; }  // Thời lượng đặt sân

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(1000, double.MaxValue, ErrorMessage = "Tổng giá trị tiền thuê sân phải lớn hơn hoặc bằng 1,000 VND.")]
        public decimal TotalPrice { get; set; }  // Tổng giá trị tiền thuê sân
        public string Status { get; set; }  // Sẽ hiển thị tên của trạng thái
        public string PaymentStatus { get; set; } // Sẽ hiển thị tên của trạng thái thanh toán

        [ForeignKey("CourtId")]
        public virtual Court Court { get; set; }  // Liên kết với bảng Courts

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }  // Liên kết với bảng Users

        [ForeignKey("StaffId")]
        public virtual StaffDetail Staff { get; set; }  // Liên kết với bảng StaffDetails


    }
}