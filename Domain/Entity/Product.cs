using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class Product : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string ThumbnailUrl { get; set; }
        public int? CategoryId { get; set; }
        [Required]
        [StringLength(255)]
        public string ProductName { get; set; }  // Tên sản phẩm
        [StringLength(255)]
        public string Description { get; set; }  // Mô tả chi tiết về sản phẩm
        [Required]
        public int Quantity { get; set; }  // Số lượng sản phẩm có sẵn
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }  // Giá sản phẩm
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }  // Liên kết với bảng Categories
    }

}