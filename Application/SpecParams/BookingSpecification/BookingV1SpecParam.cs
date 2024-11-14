using Domain.Enum;

namespace Application.SpecParams.BookingSpecification
{
    public class BookingV1SpecParam
    {
        /// <summary>
        ///  Từ ngày
        /// </summary> 
        public DateTime? FromDate { get; set; }

        /// <summary>
        ///  Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }
        /// <summary>
        ///  Trạng thái booking
        /// </summary>
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Confirmed;
        /// <summary>
        ///  Id cụm sân
        /// </summary> 
        public int CourtClusterId { get; set; }
        private string _search; // search theo cáo gì
        public string Search // getter setter
        {
            get => _search;
            set => _search = value.ToLower();
        }
    }
}