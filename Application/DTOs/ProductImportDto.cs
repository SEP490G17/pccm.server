using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ProductImportDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }  // Số lượng sản phẩm nhập
        [Required]
        [Range(1, double.MaxValue)]
        public decimal ImportFee { get; set; }  // Giá nhập sản phẩm
    }
}
