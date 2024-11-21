namespace Application.DTOs
{
    public class ProductLogDto
    {
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; } // URL ảnh đại diện cho Product
        public string ProductName { get; set; }  // Tên sản phẩm
        public int Quantity { get; set; }  // Số lượng sản phẩm có sẵn
        public decimal Price { get; set; }  // Giá sản phẩm
        public string CategoryName { get; set; }  // Liên kết với bảng Categories
        public string LogType { get; set; }
        public string CourtClusterName { get; set; }  // Liên kết với bảng Court Clusters
        public string Description { get; set; }  // Liên kết với bảng Court Clusters
        public string CreateAt { get; set; }
        public string CreateBy { get; set; }
    }

}