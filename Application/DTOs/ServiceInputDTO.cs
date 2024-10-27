using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ServiceInputDTO
    {
        public int Id { get; set; }
        public int[] CourtClusterId { get; set; }  // Mã định danh của cụm sân (có thể null)
        public string ServiceName { get; set; }  // Tên dịch vụ
        public string Description { get; set; }  // Mô tả chi tiết về dịch vụ
        public decimal Price { get; set; }  // Giá dịch vụ
    }
}