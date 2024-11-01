using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class CourtCluster : BaseEntity
    {
        [Required] [StringLength(255)] public string CourtClusterName { get; set; }  // Tên cụm sân
        [Required] public TimeOnly OpenTime { get; set; }
        [Required] public TimeOnly CloseTime { get; set; }
        [Required] [StringLength(255)] public string Location { get; set; }  // Địa điểm theo tọa độ google map của cụm sân
        [Required] [StringLength(255)] public string Address { get; set; }  // Địa điểm địa lý thực của cụm sân
        public string OwnerId { get; set; }  // Id của người sở hữu cụm sân (có thể là null)
        [Column(TypeName ="Text")] public string Description { get; set; }  // Mô tả chi tiết về cụm sân
        public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Thời gian tạo cụm sân
        [ForeignKey("OwnerId")] public virtual AppUser Owner { get; set; }  // Liên kết với bảng Users qua owner_id
        public List<Service> Services { get; set; } = new List<Service>();
        public List<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<StaffAssignment> StaffAssignments { get; set; } = new List<StaffAssignment>();
        public virtual ICollection<Court> Courts { get; set; } = new List<Court>();


    }
}