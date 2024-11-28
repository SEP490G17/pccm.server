using System.Threading.Channels;
using Application.DTOs;

namespace API.SocketSignalR
{
    public class BookingRealTimeService
    {
        private readonly Channel<(BookingDtoV1 booking, string groupId)> _createBookingChanel;
        private readonly Channel<(BookingDtoV2 booking, string groupId)> _updateStatusBookingChanel;
        public BookingRealTimeService(Channel<(BookingDtoV1 booking, string groupId)> createBookingChanel,
                                       Channel<(BookingDtoV2 booking, string groupId)> updateStatusBookingChanel)
        {
            _createBookingChanel = createBookingChanel;
            _updateStatusBookingChanel = updateStatusBookingChanel;
        }

        public async Task NotifyCreateBookingAsync(BookingDtoV1 booking, string groupId)
        {
            await _createBookingChanel.Writer.WriteAsync((booking, groupId));
        }

        public async Task NotifyUpdateBookingAsync(BookingDtoV2 booking, string groupId)
        {
            await _updateStatusBookingChanel.Writer.WriteAsync((booking, groupId));
        }

    }
}