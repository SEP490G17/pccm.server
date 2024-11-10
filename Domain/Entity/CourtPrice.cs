using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class CourtPrice : BaseEntity
    {
        [StringLength(150)] public string DisplayName { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(1000, double.MaxValue, ErrorMessage = "Giá thuê sân phải lớn hơn hoặc bằng 1,000 VND.")]
        public decimal Price { get; set; }
        public Court Court { get; set; }
        [Column(TypeName = "TIME")] public TimeOnly FromTime { get; set; }
        [Column(TypeName = "TIME")] public TimeOnly ToTime { get; set; }
    }
}