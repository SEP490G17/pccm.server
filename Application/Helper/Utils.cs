using System.Globalization;
using Domain.Entity;

namespace Application.Helper
{
    public static class Utils
    {
        static string GetDayAbbreviation(int dayNumber)
        {
            // Kiểm tra đầu vào hợp lệ
            if (dayNumber < 0 || dayNumber > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(dayNumber), "Day number must be between 0 and 6.");
            }

            // Lấy danh sách các chữ viết tắt của ngày trong tuần
            string[] abbreviations = CultureInfo.InvariantCulture
                .DateTimeFormat.AbbreviatedDayNames;

            // Chuyển về dạng SU, MO, TU, ...
            return abbreviations[dayNumber].ToUpperInvariant().Substring(0, 2);
        }
        public static decimal CalculateCourtPrice(DateTime fromTime, DateTime toTime, List<CourtPrice> courtPrices)
        {
            decimal totalPrice = 0;

            TimeZoneInfo gmtPlus7 = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");

            // Chuyển đổi từ UTC hoặc giờ hệ thống sang giờ GMT+7
            DateTime fromTimeGmt7 = TimeZoneInfo.ConvertTime(fromTime, gmtPlus7);
            DateTime toTimeGmt7 = TimeZoneInfo.ConvertTime(toTime, gmtPlus7);

            // Chuyển từ DateTime sang TimeOnly để so sánh
            TimeOnly startTimeOnly = TimeOnly.FromDateTime(fromTimeGmt7);
            TimeOnly endTimeOnly = TimeOnly.FromDateTime(toTimeGmt7);
            // Sắp xếp các mức giá theo thời gian
            courtPrices = courtPrices.OrderBy(cp => cp.FromTime).ToList();

            while (startTimeOnly < endTimeOnly)
            {
                // Tìm mức giá phù hợp với thời gian bắt đầu
                var currentPrice = courtPrices.Find(cp => startTimeOnly >= cp.FromTime && startTimeOnly < cp.ToTime);

                if (currentPrice != null)
                {
                    // Tính thời gian thuê trong khoảng giá hiện tại
                    TimeOnly nextPriceChange = endTimeOnly < currentPrice.ToTime ? endTimeOnly : currentPrice.ToTime;
                    decimal hours = (decimal)(nextPriceChange.ToTimeSpan() - startTimeOnly.ToTimeSpan()).TotalHours;
                    totalPrice += hours * currentPrice.Price;

                    // Cập nhật thời gian bắt đầu để tính cho mức giá tiếp theo
                    startTimeOnly = nextPriceChange;
                }
                else
                {
                    // Nếu không có mức giá nào phù hợp, thoát vòng lặp
                    break;
                }
            }

            return Math.Ceiling(totalPrice);
        }
    }
}