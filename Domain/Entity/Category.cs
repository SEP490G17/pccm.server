using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string CategoryName { get; set; }  // Tên danh mục sản phẩm
    }
}