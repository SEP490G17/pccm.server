using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductInputDTO
    {
        [Key]
        public int Id { get; set; }
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
    }
}
