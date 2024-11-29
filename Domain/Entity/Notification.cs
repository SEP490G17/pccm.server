

using System.Diagnostics.CodeAnalysis;
using Domain.Enum;

namespace Domain.Entity
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        [AllowNull]
        public string Url { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        [AllowNull]
        public DateTime? ReadAt { get; set; }

        public AppUser AppUser { get; set; }


    }
}