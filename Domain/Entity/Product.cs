using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entity
{
    public class Product : BaseNeedLogEntity
    {
        [StringLength(255)]
        [AllowNull]
        public string ThumbnailUrl { get; set; } // URL ảnh đại diện cho Product
        public int? CourtClusterId { get; set; }  // Mã định danh của cụm sân (có thể null)
        public int? CategoryId { get; set; }
        [Required]
        [StringLength(255)]
        public string ProductName { get; set; }  // Tên sản phẩm
        [StringLength(255)]
        public string Description { get; set; }  // Mô tả chi tiết về sản phẩm
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Quantity { get; set; }  // Số lượng sản phẩm có sẵn
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(1000, double.MaxValue, ErrorMessage = "Giá sản phẩm ít nhất phải 1,000 VND.")] public decimal Price { get; set; }  // Giá sản phẩm
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ImportFee { get; set; }
        [ForeignKey("CategoryId")]
        [AllowNull]
        public virtual Category Category { get; set; }  // Liên kết với bảng Categories
        [ForeignKey("CourtClusterId")]
        [AllowNull]
        public virtual CourtCluster CourtCluster { get; set; }  // Liên kết với bảng Court Clusters
        public virtual List<ProductLog> ProductLogs { get; set; } = new List<ProductLog>();
    }

}