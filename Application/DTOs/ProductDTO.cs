using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; } // URL ảnh đại diện cho Product
        public int? CategoryId { get; set; }
        public string ProductName { get; set; }  // Tên sản phẩm
        public int Quantity { get; set; }  // Số lượng sản phẩm có sẵn
        public decimal Price { get; set; }  // Giá sản phẩm
        public string CategoryName { get; set; }  // Liên kết với bảng Categories
        public string CourtClusterName { get; set; }  // Liên kết với bảng Court Clusters
    }
}