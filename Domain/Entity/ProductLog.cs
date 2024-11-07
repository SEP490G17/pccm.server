using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ProductLog : BaseLogEntity
    {
        public int? CourtClusterId { get; set; }  // Mã định danh của cụm sân (có thể null)
        public int? CategoryId { get; set; }
        [StringLength(255)]
        public string? ThumbnailUrl { get; set; } // URL ảnh đại diện cho Product
        [StringLength(255)]
        public string ProductName { get; set; }  // Tên sản phẩm
        [StringLength(255)]
        public string Description { get; set; }  // Mô tả chi tiết về sản phẩm
        public int Quantity { get; set; }  // Số lượng sản phẩm có sẵn
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }  // Giá sản phẩm
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }  // Liên kết với bảng Categories
        [ForeignKey("CourtClusterId")]
        public virtual CourtCluster? CourtCluster { get; set; }  // Liên kết với bảng Court Clusters

        public int ProductId { get; set; } 
    }
}