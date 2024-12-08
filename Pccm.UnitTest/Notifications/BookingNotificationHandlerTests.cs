using Application.Handler.News;
using Domain.Entity;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.Notifications;

namespace Pccm.UnitTest.Notifications
{
    [TestFixture]
    public class BookingNotificationHandlerTests
    {
        private readonly IMediator Mediator;

        public BookingNotificationHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("Thông báo đặt lịch", "Bạn đã đặt lịch thành công!", Domain.Enum.NotificationType.Booking, "https://example.com/booking-details", 14, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldShowBookingNotification_WhenValid(
            string Title,
            string Message,
            Domain.Enum.NotificationType Type,
            string Url,
            int BookingId)
        {
            try
            {
                var result = await Mediator.Send(new BookingNotification.Command()
                {
                    Title = Title,
                    Message = Message,
                    Type = Type,
                    Url = Url,
                    BookingId = BookingId
                }, default);

                // Return if the operation was successful
                if (result != null && result.NotificationDto != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase("Thông báo đặt lịch", "Bạn đã đặt lịch thành công!", Domain.Enum.NotificationType.Booking, "https://example.com/booking-details", 7, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldShowBookingNotification_WhenBookingNotExistAppUser(
            string Title,
            string Message,
            Domain.Enum.NotificationType Type,
            string Url,
            int BookingId)
        {
            try
            {
                var result = await Mediator.Send(new BookingNotification.Command()
                {
                    Title = Title,
                    Message = Message,
                    Type = Type,
                    Url = Url,
                    BookingId = BookingId
                }, default);

                // Return if the operation was successful
                if (result != null && result.NotificationDto != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
