using System.Threading.Channels;
using Application.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace API.SocketSignalR
{
    public class NotificationBackgroundService : BackgroundService
    {

        private readonly IHubContext<AppHub> _hubContext;
        private readonly Channel<(NotificationDto notification, string groupId)> _notificationChanel;
        public NotificationBackgroundService(IHubContext<AppHub> hubContext, Channel<(NotificationDto notification, string groupId)> notificationChanel)
        {
            _hubContext = hubContext;
            _notificationChanel = notificationChanel;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var (notification, groupId) in _notificationChanel.Reader.ReadAllAsync(stoppingToken))
            {
                await _hubContext.Clients.Group(groupId)
                    .SendAsync("NotificationUser", notification, stoppingToken);
            }
        }
    }
}