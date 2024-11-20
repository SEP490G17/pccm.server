using System.Globalization;

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
    }
}