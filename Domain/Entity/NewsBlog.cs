using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Domain.Entity
{
    public class NewsBlog : BaseNeedLogEntity
    {

        [Required]
        [StringLength(255)]
        public string Title { get; set; }  // Tiêu đề sự kiện

        [Required]
        [StringLength(255)]
        public string Thumbnail { get; set; } // Url Ảnh đại diện của tin tức

        [StringLength(1000)]
        public string Description { get; set; }  // Mô tả chi tiết về sự kiện

        [Column(TypeName = "TEXT")]
        public string Content { get; set; }  // Mô tả chi tiết về sự kiện

        [Required]
        public DateTime StartTime { get; set; }  // Thời gian bắt đầu sự kiện

        [Required]
        public DateTime EndTime { get; set; }  // Thời gian kết thúc sự kiện

        [Required]
        [StringLength(255)]
        public string Location { get; set; }  // Địa điểm tổ chức sự kiện

        public BannerStatus Status { get; set; }

        public string[] Tags { get; set; }

        public virtual ICollection<NewsLog> NewsLogs { get; set; } = new List<NewsLog>();

    }
}