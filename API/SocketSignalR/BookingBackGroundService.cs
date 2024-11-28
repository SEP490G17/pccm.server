


using System.Threading.Channels;
using Application.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace API.SocketSignalR
{
    public class BookingBackGroundService : BackgroundService
    {
        private readonly IHubContext<AppHub> _hubContext;
        private readonly Channel<(BookingDtoV1 booking, string groupId)> _createBookingChanel;
        private readonly Channel<(BookingDtoV2 booking, string groupId)> _updateStatusBookingChanel;
        public BookingBackGroundService(IHubContext<AppHub> hubContext, Channel<(BookingDtoV1 booking, string groupId)> createBookingChanel,
                                       Channel<(BookingDtoV2 booking, string groupId)> updateStatusBookingChanel)
        {
            _hubContext = hubContext;
            _createBookingChanel = createBookingChanel;
            _updateStatusBookingChanel = updateStatusBookingChanel;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var createBookingTask = ProcessCreateBookingsAsync(stoppingToken);
            var updateBookingTask = ProcessUpdateBookingsAsync(stoppingToken);

            await Task.WhenAny(createBookingTask, updateBookingTask);

        }

        private async Task ProcessCreateBookingsAsync(CancellationToken stoppingToken)
        {
            await foreach (var (booking, groupId) in _createBookingChanel.Reader.ReadAllAsync(stoppingToken))
            {
                await _hubContext.Clients.Group(groupId)
                    .SendAsync("CreateBooking", booking, stoppingToken);
            }
        }

        private async Task ProcessUpdateBookingsAsync(CancellationToken stoppingToken)
        {
            await foreach (var (booking, groupId) in _updateStatusBookingChanel.Reader.ReadAllAsync(stoppingToken))
            {
                await _hubContext.Clients.Group(groupId)
                   .SendAsync("UpdateBooking", booking, stoppingToken);
            }
        }
    }
}