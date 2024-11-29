
using System.Threading.Channels;
using Application.DTOs;

namespace API.SocketSignalR
{
    public class NotificationService
    {
        private readonly Channel<(NotificationDto booking, string groupId)> _notificationChanel;

        public NotificationService(Channel<(NotificationDto booking, string groupId)> notificationChanel)
        {
            _notificationChanel = notificationChanel;
        }

        public async Task NotificationForUser(NotificationDto notification, string groupId)
        {
            await _notificationChanel.Writer.WriteAsync((notification, groupId));
        }

    }
}