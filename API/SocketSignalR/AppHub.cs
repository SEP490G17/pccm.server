using Microsoft.AspNetCore.SignalR;

namespace API.SocketSignalR
{
    public class AppHub : Hub
    {
        public async Task JoinClusterGroup(string clusterId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, clusterId);
        }

        public async Task LeaveClusterGroup(string clusterId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, clusterId);
        }
    }
}