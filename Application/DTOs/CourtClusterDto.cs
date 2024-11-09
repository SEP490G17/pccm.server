using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

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
            public int Id { get; set; }
            public string Title { get; set; }  // Tên cụm sân
            public string Address { get; set; }  // Địa điểm địa lý thực của cụm sân
            public int NumbOfCourts { get; set; }
            public string Description { get; set; }
            public TimeOnly OpenTime { get; set; }
            public TimeOnly CloseTime { get; set; }
            public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        }

        public class CourtCLusterListPageUserSite
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string CourtClusterName { get; set; }
            public string Address { get; set; }
            public string Province { get; set; }  //Tỉnh thành
            public string ProvinceName { get; set; }
            public string District { get; set; }  // Thành phố, quận 
            public string DistrictName { get; set; }
            public string Ward { get; set; }  // Phường
            public string WardName { get; set; }
            public int NumbOfCourts { get; set; }
            public int Rate { get; set; }
            public string Price { get; set; }
            public string Description { get; set; }
            public TimeOnly OpenTime { get; set; }
            public TimeOnly CloseTime { get; set; }
            public string[] Images { get; set; }
            public List<ServiceDto> Services { get; set; } = new List<ServiceDto>();
            public List<ProductDto> Products { get; set; } = new List<ProductDto>();
            public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();

        }

        public class CourtCLusterDetails
        {
            public int Id { get; set; }
            public string Title { get; set; }  // Tên cụm sân
            public string Address { get; set; }  // Địa điểm địa lý thực của cụm sân
            public string Location { get; set; }  // Địa điểm địa lý thực của cụm sân
            public int NumbOfCourts { get; set; }
            public string Description { get; set; }
            public TimeOnly OpenTime { get; set; }
            public TimeOnly CloseTime { get; set; }
            public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        }

    }
}