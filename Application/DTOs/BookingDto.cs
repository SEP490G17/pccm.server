using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class BookingDto
    {
        [Key]
        public int Id { get; set; }
        public string CourtName { get; set; }

        public string UserName { get; set; }

        public DateTime StartTime { get; set; }  // Thời gian bắt đầu đặt sân

        public DateTime EndTime { get; set; }  // Thời gian kết thúc đặt sân

        public int Duration { get; set; }  // Thời lượng đặt sân

        public decimal TotalPrice { get; set; }  // Tổng giá trị tiền thuê sân
        public string Status { get; set; }  // Sẽ hiển thị tên của trạng thái
        public string PaymentStatus { get; set; } // Sẽ hiển thị tên của trạng thái thanh toán

    }
}