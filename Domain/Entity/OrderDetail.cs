using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class OrderDetail : BaseEntity
    {
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? ServiceId { get; set; }
        [Required]
        public decimal Quantity { get; set; }  // Số lượng sản phẩm hoặc dịch vụ
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPrice { get; set; }  // Giá của sản phẩm hoặc dịch vụ
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }  // Liên kết với bảng Orders
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }  // Liên kết với bảng Products
        [ForeignKey("ServiceId")]
        public virtual Service? Service { get; set; }  // Liên kết với bảng Services
    }

}