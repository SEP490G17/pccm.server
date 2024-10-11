using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CourtClustersInputDTO
    {
         [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string CourtClusterName { get; set; }  // Tên cụm sân
        [Required]
        [StringLength(255)]
        public string Location { get; set; }  // Địa điểm theo tọa độ google map của cụm sân
        public string OwnerId { get; set; }  // Id của người sở hữu cụm sân (có thể là null)
        [StringLength(255)]
        public string Description { get; set; }  // Mô tả chi tiết về cụm sân
        public string Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Thời gian tạo cụm sân
        
    }
}