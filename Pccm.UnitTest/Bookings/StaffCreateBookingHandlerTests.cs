using Application.Handler.Bookings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class StaffCreateBookingHandlerTests
    {
        private readonly IMediator Mediator;

        public StaffCreateBookingHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("123456789", "John Doe", 20, "2025-11-11T10:00:00", "2025-11-11T12:00:00", null, null, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCreateBooking_WhenValidData(
            string PhoneNumber,
            string FullName,
            int CourtId,
            string StartTime,
            string EndTime,
            string? UntilTime,
            string? RecurrenceRule)
        {
            try
            {
                var bookingInputDto = new BookingInputDto()
                {
                    PhoneNumber = PhoneNumber,
                    FullName = FullName,
                    CourtId = CourtId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    UntilTime = string.IsNullOrEmpty(UntilTime) ? (DateTime?)null : DateTime.Parse(UntilTime),
                    RecurrenceRule = RecurrenceRule
                };

                var result = await Mediator.Send(new StaffCreate.Command() { Booking = bookingInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(null, "John Doe", 1, "2024-11-11T10:00:00", "2024-11-11T12:00:00", null, "Weekly", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateBookingFail_WhenPhoneNumberIsNull(
            string? PhoneNumber,
            string FullName,
            int CourtId,
            string StartTime,
            string EndTime,
            string? UntilTime,
            string RecurrenceRule)
        {
            try
            {
                var bookingInputDto = new BookingInputDto()
                {
                    PhoneNumber = PhoneNumber,
                    FullName = FullName,
                    CourtId = CourtId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    UntilTime = string.IsNullOrEmpty(UntilTime) ? (DateTime?)null : DateTime.Parse(UntilTime),
                    RecurrenceRule = RecurrenceRule
                };

                var result = await Mediator.Send(new StaffCreate.Command() { Booking = bookingInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("123456789", null, 1, "2024-11-11T10:00:00", "2024-11-11T12:00:00", null, "Weekly", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateBookingFail_WhenFullNameIsNull(
            string PhoneNumber,
            string? FullName,
            int CourtId,
            string StartTime,
            string EndTime,
            string? UntilTime,
            string RecurrenceRule)
        {
            try
            {
                var bookingInputDto = new BookingInputDto()
                {
                    PhoneNumber = PhoneNumber,
                    FullName = FullName,
                    CourtId = CourtId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    UntilTime = string.IsNullOrEmpty(UntilTime) ? (DateTime?)null : DateTime.Parse(UntilTime),
                    RecurrenceRule = RecurrenceRule
                };

                var result = await Mediator.Send(new StaffCreate.Command() { Booking = bookingInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("123456789", "John Doe", 1111, "2024-11-11T10:00:00", "2024-11-11T12:00:00", null, "Weekly", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateBookingFail_WhenCourtIDNotExist(
           string PhoneNumber,
           string FullName,
           int CourtId,
           string? StartTime,
           string EndTime,
           string? UntilTime,
           string RecurrenceRule)
        {
            try
            {
                var bookingInputDto = new BookingInputDto()
                {
                    PhoneNumber = PhoneNumber,
                    FullName = FullName,
                    CourtId = CourtId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    UntilTime = string.IsNullOrEmpty(UntilTime) ? (DateTime?)null : DateTime.Parse(UntilTime),
                    RecurrenceRule = RecurrenceRule
                };

                var result = await Mediator.Send(new StaffCreate.Command() { Booking = bookingInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("123456789", "John Doe", 1, null, "2024-11-11T12:00:00", null, "Weekly", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateBookingFail_WhenStartTimeIsNull(
            string PhoneNumber,
            string FullName,
            int CourtId,
            string? StartTime,
            string EndTime,
            string? UntilTime,
            string RecurrenceRule)
        {
            try
            {
                var bookingInputDto = new BookingInputDto()
                {
                    PhoneNumber = PhoneNumber,
                    FullName = FullName,
                    CourtId = CourtId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    UntilTime = string.IsNullOrEmpty(UntilTime) ? (DateTime?)null : DateTime.Parse(UntilTime),
                    RecurrenceRule = RecurrenceRule
                };

                var result = await Mediator.Send(new StaffCreate.Command() { Booking = bookingInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("123456789", "John Doe", 1, "2024-11-11T10:00:00", null, null, "Weekly", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateBookingFail_WhenEndTimeIsNull(
            string PhoneNumber,
            string FullName,
            int CourtId,
            string StartTime,
            string? EndTime,
            string? UntilTime,
            string RecurrenceRule)
        {
            try
            {
                var bookingInputDto = new BookingInputDto()
                {
                    PhoneNumber = PhoneNumber,
                    FullName = FullName,
                    CourtId = CourtId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    UntilTime = string.IsNullOrEmpty(UntilTime) ? (DateTime?)null : DateTime.Parse(UntilTime),
                    RecurrenceRule = RecurrenceRule
                };

                var result = await Mediator.Send(new StaffCreate.Command() { Booking = bookingInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("123456789", "John Doe", 1, "2024-09-09", null, null, "Weekly", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateBookingFail_WhenBookingDateBeforeCurrentDate(
           string PhoneNumber,
           string FullName,
           int CourtId,
           string StartTime,
           string? EndTime,
           string? UntilTime,
           string RecurrenceRule)
        {
            try
            {
                var bookingInputDto = new BookingInputDto()
                {
                    PhoneNumber = PhoneNumber,
                    FullName = FullName,
                    CourtId = CourtId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    UntilTime = string.IsNullOrEmpty(UntilTime) ? (DateTime?)null : DateTime.Parse(UntilTime),
                    RecurrenceRule = RecurrenceRule
                };

                var result = await Mediator.Send(new StaffCreate.Command() { Booking = bookingInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("123456789", "John Doe", 1, "2024-11-11", null, null, "Weekly", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateBookingFail_WhenDuplicateBooking(
             string PhoneNumber,
            string FullName,
            int CourtId,
            string StartTime,
            string? EndTime,
            string? UntilTime,
            string RecurrenceRule)
        {
             try
            {
                var bookingInputDto = new BookingInputDto()
                {
                    PhoneNumber = PhoneNumber,
                    FullName = FullName,
                    CourtId = CourtId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    UntilTime = string.IsNullOrEmpty(UntilTime) ? (DateTime?)null : DateTime.Parse(UntilTime),
                    RecurrenceRule = RecurrenceRule
                };

                var result = await Mediator.Send(new StaffCreate.Command() { Booking = bookingInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase("123456789", "John Doe", 1, "2024-11-29", null, null, "Weekly", ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateBookingFail_WhenDuplicateBookingCombo(
             string PhoneNumber,
            string FullName,
            int CourtId,
            string StartTime,
            string? EndTime,
            string? UntilTime,
            string RecurrenceRule)
        {
             try
            {
                var bookingInputDto = new BookingInputDto()
                {
                    PhoneNumber = PhoneNumber,
                    FullName = FullName,
                    CourtId = CourtId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    UntilTime = string.IsNullOrEmpty(UntilTime) ? (DateTime?)null : DateTime.Parse(UntilTime),
                    RecurrenceRule = RecurrenceRule
                };

                var result = await Mediator.Send(new StaffCreate.Command() { Booking = bookingInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
