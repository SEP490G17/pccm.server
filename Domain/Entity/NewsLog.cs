using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Domain.Entity
{
    public class NewsLog : BaseLogEntity
    {
        [StringLength(255)]
        public string Title { get; set; }  // Tiêu đề sự kiện

        [StringLength(255)]
        public string Thumbnail { get; set; } // Url Ảnh đại diện của tin tức

        [StringLength(1000)]
        public string Description { get; set; }  // Mô tả chi tiết về sự kiện

        [Column(TypeName = "TEXT")]
        public string Content { get; set; }  // Mô tả chi tiết về sự kiện

        public DateTime StartTime { get; set; }  // Thời gian bắt đầu sự kiện

        public DateTime EndTime { get; set; }  // Thời gian kết thúc sự kiện


        public string Location { get; set; }  // Địa điểm tổ chức sự kiện

        public BannerStatus Status { get; set; }

        public string[] Tags { get; set; }

        public int NewsBlogId { get; set; }

        public LogType LogType { get; set; }
    }
}