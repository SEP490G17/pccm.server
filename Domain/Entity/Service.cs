using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class Service : BaseEntity
    {
        public int? CourtClusterId { get; set; }  // Mã định danh của cụm sân (có thể null)
        [Required]
        [StringLength(255)]
        public string ServiceName { get; set; }  // Tên dịch vụ
        [StringLength(255)]
        public string Description { get; set; }  // Mô tả chi tiết về dịch vụ
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }  // Giá dịch vụ
        [ForeignKey("CourtClusterId")]
        public virtual CourtCluster CourtCluster { get; set; }  // Liên kết với bảng Court Clusters
    }
}