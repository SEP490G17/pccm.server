using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Event
    {
        public int Id { get; set; } 

        [Required]
        [StringLength(255)]
        public string Title { get; set; }  // Tiêu đề sự kiện

        [StringLength(255)]
        public string Description { get; set; }  // Mô tả chi tiết về sự kiện

        [Required]
        public DateTime StartTime { get; set; }  // Thời gian bắt đầu sự kiện

        [Required]
        public DateTime EndTime { get; set; }  // Thời gian kết thúc sự kiện

        [Required]
        [StringLength(255)]
        public string Location { get; set; }  // Địa điểm tổ chức sự kiện

        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Thời gian tạo sự kiện
    }
}