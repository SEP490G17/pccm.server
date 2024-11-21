namespace Application.DTOs
{
    public class ServiceLogDto
    {
        public int Id { get; set; }
        public string CourtClusterName { get; set; }
        public string ServiceName { get; set; }  // Tên dịch vụ
        public string Description { get; set; }  // Mô tả chi tiết về dịch vụ
        public decimal Price { get; set; }  // Giá dịch vụ
        public string LogType { get; set; }
        public string CreateAt { get; set; }
        public string CreateBy { get; set; }
    }
}
