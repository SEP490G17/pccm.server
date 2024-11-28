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
            public bool IsVisible { get; set; }
            public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
        }

        public class CourtClusterListPageUserSite
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
            public TimeOnly? OpenTime { get; set; }
            public TimeOnly? CloseTime { get; set; }
            public string[] Images { get; set; }
            public List<CourtOfClusterDto> Courts { get; set; }

        }

        public class CourtClusterDetails
        {
            public int Id { get; set; }
            public string Title { get; set; }  // Tên cụm sân
            public string Address { get; set; }  // Địa điểm địa lý thực của cụm sân
            public string Province { get; set; }  //Tỉnh thành
            public string ProvinceName { get; set; }
            public string District { get; set; }  // Thành phố, quận 
            public string DistrictName { get; set; }
            public string Ward { get; set; }  // Phường
            public string WardName { get; set; }

            public string Location { get; set; }  // Địa điểm địa lý thực của cụm sân
            public int NumbOfCourts { get; set; }
            public string Description { get; set; }
            public TimeOnly OpenTime { get; set; }
            public TimeOnly CloseTime { get; set; }
            public decimal MinPrice { get; set; }
            public decimal MaxPrice { get; set; }
            public string[] Images { get; set; }  // Lưu danh sách ảnh dưới dạng JSON
            public List<CourtOfClusterDto> Courts { get; set; }

        }

    }
}