using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Domain.Entity
{
    public class Banner : BaseNeedLogEntity
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }  // Tiêu đề của banner

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }  // Đường dẫn đến hình ảnh của banner
        public BannerStatus Status { get; set; } // Trạng thái hiển thị
        public BannerType BannerType { get; set; } = BannerType.Banner; // Loại banner
        public BannerInPage BannerInPage { get; set; } = BannerInPage.HomePage; // ở trang nào

        [Column(TypeName = "LONGTEXT")]
        public string Description { get; set; }  // Mô tả ngắn của banner

        [StringLength(255)]
        public string LinkUrl { get; set; }  // Đường dẫn khi người dùng click vào banner
        public DateTime StartDate { get; set; }  // Ngày bắt đầu hiển thị banner
        public DateTime EndDate { get; set; }  // Ngày kết thúc hiển thị banner

        public virtual ICollection<BannerLog> BannerLogs { get; set; } = new List<BannerLog>();  // Danh sách các log banner này
    }
}