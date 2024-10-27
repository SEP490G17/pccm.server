using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Domain.Entity
{
    public class Court : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string CourtName { get; set; }  // Tên sân

        public int? CourtClusterId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(1000, double.MaxValue, ErrorMessage = "Giá thuê sân phải lớn hơn hoặc bằng 1,000 VND.")]
        public decimal PricePerHour { get; set; }  // Giá thuê sân theo giờ

        [Required]
        public CourtStatus Status { get; set; }  // Trạng thái của sân
        [ForeignKey("CourtClusterId")]
        public virtual CourtCluster? CourtCluster { get; set; }
        public ICollection<CourtPrice> CourtPrices { get; set; } = new List<CourtPrice>();
    }
}