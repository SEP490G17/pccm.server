using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entity
{
    public class CourtCluster : BaseEntity
    {
        [Required][StringLength(255)] public string CourtClusterName { get; set; }  // Tên cụm sân
        [Required] public TimeOnly OpenTime { get; set; }
        [Required] public TimeOnly CloseTime { get; set; }
        [Required][StringLength(10)] public string Province { get; set; }  //Tỉnh thành
        [Required][StringLength(255)] public string ProvinceName { get; set; }
        [Required][StringLength(10)] public string District { get; set; }  // Thành phố, quận 
        [Required][StringLength(255)] public string DistrictName { get; set; }
        [Required][StringLength(255)] public string Ward { get; set; }  // Phường
        [Required][StringLength(255)] public string WardName { get; set; }
        [Required][StringLength(255)] public string Address { get; set; }  // Địa điểm cụ thể
        public string OwnerId { get; set; }  // Id của người sở hữu cụm sân (có thể là null)
        [Column(TypeName = "LONGTEXT")] public string Description { get; set; }  // Mô tả chi tiết về cụm sân
        public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Thời gian tạo cụm sân
        [ForeignKey("OwnerId")] public virtual AppUser Owner { get; set; }  // Liên kết với bảng Users qua owner_id
        public DateTime? DeleteAt { get; set; }
        [AllowNull]
        public AppUser DeleteBy { get; set; }
        [Required] public bool IsVisible { get; set; } = true;
        public List<Service> Services { get; set; } = new List<Service>();
        public List<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<StaffAssignment> StaffAssignments { get; set; } = new List<StaffAssignment>();
        public virtual ICollection<Court> Courts { get; set; } = new List<Court>();
        
        // Navigation property for Reviews
        public ICollection<Review> Reviews { get; set; } = new List<Review>();


    }
}