using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class BookingInputDto
    {
        public string PhoneNumber { get; set; }

        public int CourtId { get; set; }
        public int CourtClusterId { get; set; }

        public DateTime StartTime { get; set; }  // Thời gian bắt đầu đặt sân

        public DateTime EndTime { get; set; }  // Thời gian kết thúc đặt sân

        public string  RecurrenceRule { get; set; }  // Thời lượng đặt sân
    }
}