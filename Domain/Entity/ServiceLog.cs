using Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class ServiceLog:BaseLogEntity
    {
        [StringLength(255)]
        public string ServiceName { get; set; }  // Tên dịch vụ
        [StringLength(255)]
        public string Description { get; set; }  // Mô tả chi tiết về dịch vụ
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }  // Giá dịch vụ
        public virtual CourtCluster CourtCluster { get; set; }  // Liên kết với bảng Court Clusters
        public int ServiceId { get; set; }
        public LogType LogType { get; set; }
    }
}