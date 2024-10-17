using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Category : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string CategoryName { get; set; }  // Tên danh mục sản phẩm
    }
}