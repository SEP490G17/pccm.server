using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class GetSlot
    {
        public class Query : IRequest<Result<AvailableSlotDto.AvailableSlotsResponse>>
        {
            public string Date { get; set; }
            public int CourtClusterId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<AvailableSlotDto.AvailableSlotsResponse>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<AvailableSlotDto.AvailableSlotsResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new AvailableSlotDto.AvailableSlotsResponse();

                // Parse the input date
                if (!DateTime.TryParse(request.Date, out var parsedDate))
                {
                    return Result<AvailableSlotDto.AvailableSlotsResponse>.Failure("Invalid date format. Please provide a valid date.");
                }

                // Fetch court cluster information
                var courtCluster = await _context.CourtClusters
                    .FirstOrDefaultAsync(c => c.Id == request.CourtClusterId, cancellationToken);

                if (courtCluster == null)
                {
                    return Result<AvailableSlotDto.AvailableSlotsResponse>.Failure("Court cluster not found.");
                }

                // Define open and close times
                var openTime = parsedDate.Date.Add(courtCluster.OpenTime.ToTimeSpan());
                var closeTime = parsedDate.Date.Add(courtCluster.CloseTime.ToTimeSpan());

                // Fetch all courts within the cluster
                var courts = await _context.Courts
                    .Where(c => c.CourtClusterId == request.CourtClusterId && c.DeleteAt == null)
                    .ToListAsync(cancellationToken);

                foreach (var court in courts)
                {
                    // Fetch bookings for the court on the specified date
                    var bookings = await _context.Bookings
                        .Where(b =>
                            b.Court.Id == court.Id &&
                            b.Status != BookingStatus.Cancelled &&
                            b.StartTime.Date <= parsedDate.Date &&
                            (b.UntilTime == null || b.UntilTime.Value.Date >= parsedDate.Date))
                        .OrderBy(b => b.StartTime)
                        .ToListAsync(cancellationToken);

                    // Process bookings and calculate available slots
                    var availableSlots = CalculateAvailableSlots(bookings, openTime, closeTime, parsedDate);

                    // Add results to response
                    response.AvailableSlots.Add(new AvailableSlotDto.CourtAvailableSlot
                    {
                        Id = court.Id,
                        Name = court.CourtName,
                        AvailableSlots = availableSlots
                    });
                }

                return Result<AvailableSlotDto.AvailableSlotsResponse>.Success(response);
            }

            private List<string> CalculateAvailableSlots(List<Booking> bookings, DateTime openTime, DateTime closeTime, DateTime targetDate)
            {
                var availableSlots = new List<string>();
                var currentTime = openTime;

                // Xử lý từng booking
                foreach (var booking in bookings)
                {
                    // Kiểm tra nếu booking có UntilTime và áp dụng logic lặp lại
                    if (booking.UntilTime != null && booking.UntilTime.Value.Date >= targetDate.Date)
                    {
                        // Tạo khoảng thời gian booking lặp lại trong ngày targetDate
                        var repeatedStartTime = targetDate.Date.Add(booking.StartTime.TimeOfDay);
                        var repeatedEndTime = targetDate.Date.Add(booking.EndTime.TimeOfDay);

                        // Nếu khoảng thời gian lặp lại nằm sau currentTime, tính giờ trống
                        if (repeatedStartTime > currentTime)
                        {
                            availableSlots.Add($"{currentTime:HH\\:mm} - {repeatedStartTime.AddHours(7):HH\\:mm}");
                        }

                        // Cập nhật currentTime tới thời gian kết thúc booking
                        currentTime = repeatedEndTime > currentTime ? repeatedEndTime.AddHours(7) : currentTime.AddHours(7);
                    }
                    else if (booking.StartTime.Date == targetDate.Date)
                    {
                        // Nếu booking không lặp lại, xử lý như bình thường
                        if (booking.StartTime > currentTime)
                        {
                            availableSlots.Add($"{currentTime:HH\\:mm} - {booking.StartTime.AddHours(7):HH\\:mm}");
                        }

                        currentTime = booking.EndTime > currentTime ? booking.EndTime.AddHours(7) : currentTime.AddHours(7);
                    }
                }

                // Kiểm tra giờ trống sau booking cuối cùng đến closeTime
                if (currentTime < closeTime)
                {
                    availableSlots.Add($"{currentTime:HH\\:mm} - {closeTime:HH\\:mm}");
                }

                return availableSlots;
            }
        }
    }
}
