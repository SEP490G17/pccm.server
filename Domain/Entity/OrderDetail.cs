using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entity
{
    public class OrderDetail : BaseEntity
    {
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? ServiceId { get; set; }
        [Required]
        public double Quantity { get; set; }  // Số lượng sản phẩm hoặc dịch vụ
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }  // Giá của sản phẩm hoặc dịch vụ
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }  // Liên kết với bảng Orders
        [ForeignKey("ProductId")]
        [AllowNull]
        public virtual Product Product { get; set; }  // Liên kết với bảng Products
        [ForeignKey("ServiceId")]
        [AllowNull]
        public virtual Service Service { get; set; }  // Liên kết với bảng Services
    }

}