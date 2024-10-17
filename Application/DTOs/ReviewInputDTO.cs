using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ReviewInputDTO
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? CourtClusterId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }  // Điểm đánh giá từ 1 đến 5
        public string Comment { get; set; }  // Bình luận chi tiết của người dùng
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Thời gian đánh giá được tạo
    }
}
