using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CourtClusterDto
    {
        public class CourtClusterListAll
        {
            public int Id { get; set; }
            public string CourtClusterName { get; set; }
        }

        public class CourtCLusterListPage
        {
            public string CourtClusterName { get; set; }  // Tên cụm sân
            public string Location { get; set; }  // Địa điểm theo tọa độ google map của cụm sân
            public string Address { get; set; }  // Địa điểm địa lý thực của cụm sân
            public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        }
        
    }
}