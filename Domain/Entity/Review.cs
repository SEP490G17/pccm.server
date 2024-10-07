using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? CourtClusterId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }  // Điểm đánh giá từ 1 đến 5
        public string Comment { get; set; }  // Bình luận chi tiết của người dùng
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Thời gian đánh giá được tạo
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }  // Liên kết với bảng Users
        [ForeignKey("CourtClusterId")]
        public virtual CourtCluster CourtCluster { get; set; }  // Liên kết với bảng Court Clusters
    }
}