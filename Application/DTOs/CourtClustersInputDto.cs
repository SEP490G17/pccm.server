using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CourtClustersInputDto
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }  // Tên cụm sân
        [Required][StringLength(10)] public string Province { get; set; }  //Tỉnh thành
        [Required][StringLength(255)] public string ProvinceName { get; set; }
        [Required][StringLength(10)] public string District { get; set; }  // Thành phố, quận 
        [Required][StringLength(255)] public string DistrictName { get; set; }
        [Required][StringLength(255)] public string Ward { get; set; }  // Phường
        [Required][StringLength(255)] public string WardName { get; set; }
        [Required]
        [StringLength(255)]
        public string Address { get; set; }  // Địa điểm địa lý thực của cụm sân
        public string OwnerId { get; set; }  // Id của người sở hữu cụm sân (có thể là null)
        [StringLength(255)]
        public string Description { get; set; }  // Mô tả chi tiết về cụm sân
        public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Thời gian tạo cụm sân

    }

    public class CourtDetailsDto
    {
        public string CourtName { get; set; }
        public CourtPricesDto[] CourtPrice { get; set; }
        public int Status { get; set; }
        public string Actions { get; set; }
    }

    public class CourtPricesDto
    {
        public decimal Price { get; set; }
        public TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }
    }

    public enum CourtActions
    {
        NONE,
        ADD,
        UPDATE,
        DELETE
    }
}

