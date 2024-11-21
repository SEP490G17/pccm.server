using Domain.Enum;

namespace Application.DTOs
{
    public class CourtClustersInputDto
    {
        public string Title { get; set; }  // Tên cụm sân
        public string Province { get; set; }  //Tỉnh thành
        public string ProvinceName { get; set; }
        public string District { get; set; }  // Thành phố, quận 
        public string DistrictName { get; set; }
        public string Ward { get; set; }  // Phường
        public string WardName { get; set; }

        public string Address { get; set; }  // Địa điểm địa lý thực của cụm sân
        public string OwnerId { get; set; }  // Id của người sở hữu cụm sân (có thể là null)
        public string Description { get; set; }  // Mô tả chi tiết về cụm sân
        public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Thời gian tạo cụm sân

        public List<CourtDetailsDto> CourtDetails { get; set; }



    }

    public class CourtDetailsDto
    {
        public string CourtName { get; set; }
        public List<CourtPricesDto> CourtPrice { get; set; }
        public CourtStatus Status { get; set; }
        public string? Actions { get; set; }
    }

    public class CourtPricesDto
    {
        public decimal Price { get; set; }
        public TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }
    }
    public class CourtClustersEditInput
    {
        public string Title { get; set; }  // Tên cụm sân
        public string Province { get; set; }  //Tỉnh thành
        public string ProvinceName { get; set; }
        public string District { get; set; }  // Thành phố, quận 
        public string DistrictName { get; set; }
        public string Ward { get; set; }  // Phường
        public string WardName { get; set; }

        public string Address { get; set; }  // Địa điểm địa lý thực của cụm sân
        public string OwnerId { get; set; }  // Id của người sở hữu cụm sân (có thể là null)
        public string Description { get; set; }  // Mô tả chi tiết về cụm sân
        public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }

    }

    public enum CourtActions
    {
        NONE,
        ADD,
        UPDATE,
        DELETE
    }
}

