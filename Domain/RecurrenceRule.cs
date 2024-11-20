

namespace Domain
{
    /// <summary>
    /// lấy recurrenceRule kiểu 
    /// - Recurrence Rule Changed: FREQ=WEEKLY;BYDAY=SU,MO,TU,WE,TH,FR,SA;INTERVAL=1;UNTIL=20250116T142546Z;
    //  - Recurrence Rule Changed: FREQ=DAILY;                            INTERVAL=1;UNTIL=20250116T142546Z;
    /// </summary>
    public class RecurrenceRule
    {
        public string Frequency { get; set; } // FREQ
        public int Interval { get; set; } = 1; // INTERVAL (Default: 1)
        public List<string> ByDays { get; set; } = new(); // BYDAY
        public DateTime? Until { get; set; } // UNTIL (UTC)

        /// <summary>
        /// Lấy chuỗi luật
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var ruleParts = new List<string>
        {
            $"FREQ={Frequency}",
            $"INTERVAL={Interval}" // Interval is fixed
        };

            if (ByDays != null && ByDays.Any())
                ruleParts.Add($"BYDAY={string.Join(",", ByDays)}");

            if (Until.HasValue)
            {
                var untilDateTime = Until.Value;
                if (untilDateTime.Kind != DateTimeKind.Utc)
                {
                    untilDateTime = untilDateTime.ToUniversalTime();
                }

                ruleParts.Add($"UNTIL={untilDateTime.ToString("yyyyMMdd'T'HHmmss'Z'")}");
            }

            return string.Join(";", ruleParts);
        }

        public static RecurrenceRule FromString(string rule)
        {
            var recurrence = new RecurrenceRule();
            var parts = rule.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var kv = part.Split('=');
                if (kv.Length != 2) continue;

                switch (kv[0])
                {
                    case "FREQ":
                        recurrence.Frequency = kv[1];
                        break;
                    case "INTERVAL":
                        recurrence.Interval = int.Parse(kv[1]);
                        break;
                    case "BYDAY":
                        recurrence.ByDays = kv[1].Split(',').ToList();
                        break;
                    case "UNTIL":
                        recurrence.Until = DateTime.ParseExact(kv[1], "yyyyMMdd'T'HHmmss'Z'", null);
                        break;
                }
            }

            return recurrence;
        }
    }
}