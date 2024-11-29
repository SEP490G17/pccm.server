using Domain.Enum;

namespace Application.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public string? Url { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class NotificationQueryResult{
        public NotificationDto NotificationDto{ get; set; }
        public string PhoneNumber { get; set; }
    }
}