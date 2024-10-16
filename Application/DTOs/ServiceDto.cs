using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ServiceDto
    {
        [Key]
        public int Id { get; set; }
        public int? CourtClusterId { get; set; }  // Mã định danh của cụm sân (có thể null)
        [Required]
        [StringLength(255)]
        public string ServiceName { get; set; }  // Tên dịch vụ
        [StringLength(255)]
        public string Description { get; set; }  // Mô tả chi tiết về dịch vụ
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }  // Giá dịch vụ
    }
}
