namespace Application.DTOs
{
    public class BillDto
    {
        public BookingBillDto booking { get; set; }
        public CourtClusterBillDto courtCluster { get; set; }
        public List<ProductBillDto>? products { get; set; }
        public List<ServiceBillDto>? services { get; set; }
    }

    public class ProductBillDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ServiceBillDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class BookingBillDto
    {
        public int Id { get; set; }
        public string CourtName { get; set; }

        public DateTime StartTime { get; set; }  // Thời gian bắt đầu đặt sân

        public DateTime EndTime { get; set; }  // Thời gian kết thúc đặt sân

        public decimal TotalPrice { get; set; }  // Tổng giá trị tiền thuê sân
    }

    public class CourtClusterBillDto
    {
        public int Id { get; set; }
        public string CourtClusterName { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string ProvinceName { get; set; }
        public string District { get; set; }  // Thành phố, quận 
        public string DistrictName { get; set; }
        public string Ward { get; set; }  // Phường
        public string WardName { get; set; }
    }
}