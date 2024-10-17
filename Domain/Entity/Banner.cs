using System.ComponentModel.DataAnnotations;
using Domain.Enum;

namespace Domain.Entity
{
    public class Banner : BaseEntity
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

        [StringLength(255)]
        public string Description { get; set; }  // Mô tả ngắn của banner

        [StringLength(255)]
        public string LinkUrl { get; set; }  // Đường dẫn khi người dùng click vào banner
        public DateTime StartDate { get; set; }  // Ngày bắt đầu hiển thị banner
        public DateTime EndDate { get; set; }  // Ngày kết thúc hiển thị banner
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Thời gian banner được tạo
    }
}