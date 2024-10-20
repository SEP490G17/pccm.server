using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class StaffDetail : BaseEntity
    {
        public string UserId { get; set; }  // Id của người dùng 
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(1000, double.MaxValue, ErrorMessage = "Lương phải lớn hơn hoặc bằng 1,000 VND.")]
        public decimal Salary { get; set; }  // Mức lương của nhân viên
        public int? ShiftId { get; set; }  // Id của ca làm việc (có thể là null)
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }  // Liên kết với bảng Users

        public virtual StaffPosition Position { get; set; } 
    }
}