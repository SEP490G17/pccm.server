using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class BannerDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }  // Tiêu đề của banner
        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }  // Đường dẫn đến hình ảnh của banner
        [StringLength(255)]
        public string LinkUrl { get; set; }  // Đường dẫn khi người dùng click vào banner
        public DateTime StartDate { get; set; }  // Ngày bắt đầu hiển thị banner
        public DateTime EndDate { get; set; }  // Ngày kết thúc hiển thị banner
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Thời gian banner được tạo
    }
}