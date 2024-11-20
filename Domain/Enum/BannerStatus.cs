namespace Domain.Enum
{
    public enum BannerStatus
    {
        Hidden = 0,
        Display = 1
    }
    public static class BannerStatusExtensions
    {
        public static string ToFriendlyString(this BannerStatus status)
        {
            switch (status)
            {
                case BannerStatus.Display:
                    return "hoạt động";
                case BannerStatus.Hidden:
                    return "không hoạt động";
                default:
                    return "không xác định";
            }
        }
    }

}