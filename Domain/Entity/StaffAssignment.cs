using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class StaffAssignment
    {
        [Key]
        public int Id { get; set; } 
        public int StaffId { get; set; }  // Mã định danh của nhân viên
        public int CourtClusterId { get; set; }  // Mã định danh của cụm sân
        [ForeignKey("StaffId")]
        public virtual StaffDetail StaffDetail { get; set; }  // Liên kết với bảng StaffDetails
        [ForeignKey("CourtClusterId")]
        public virtual CourtCluster CourtCluster { get; set; }  // Liên kết với bảng Court Clusters
    }
}