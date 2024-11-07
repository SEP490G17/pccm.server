using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
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
                this._context = context;
                this._mapper = mapper;
            }
          public async Task<Result<AvailableSlotDto.AvailableSlotsResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new AvailableSlotDto.AvailableSlotsResponse();

                // Parse the date from request.Date (which is a string)
                if (!DateTime.TryParse(request.Date, out var parsedDate))
                {
                    throw new Exception("Invalid date format. Please provide a valid date.");
                }

                // Fetch the Court Cluster details
                var courtCluster = await _context.CourtClusters
                    .FirstOrDefaultAsync(c => c.Id == request.CourtClusterId, cancellationToken);

                if (courtCluster == null) throw new Exception("Court cluster not found.");

                var openTime = parsedDate.Date.Add(courtCluster.OpenTime.ToTimeSpan());
                var closeTime = parsedDate.Date.Add(courtCluster.CloseTime.ToTimeSpan());

                // Fetch all courts in the specified CourtCluster
                var courts = await _context.Courts
                    .Where(c => c.CourtClusterId == request.CourtClusterId)
                    .ToListAsync(cancellationToken);

                foreach (var court in courts)
                {
                    // Fetch bookings for the court on the specified date, including those with recurrence
                    var bookings = await _context.Bookings
                        .Where(b => b.Court.Id == court.Id &&
                                    (b.StartTime.Date == parsedDate.Date || b.RecurrenceRule != null))
                        .OrderBy(b => b.StartTime)
                        .ToListAsync(cancellationToken);

                    var availableTimes = new List<string>();
                    var currentTime = openTime;

                    var expandedBookings = ExpandRecurringBookings(bookings, parsedDate, openTime, closeTime);

                    foreach (var booking in expandedBookings)
                    {
                        if (booking.StartTime > currentTime)
                        {
                            availableTimes.Add($"{currentTime.TimeOfDay:hh\\:mm} - {booking.StartTime.TimeOfDay:hh\\:mm}");
                        }
                        currentTime = booking.EndTime;
                    }

                    // Check for availability after the last booking
                    if (currentTime < closeTime)
                    {
                        availableTimes.Add($"{currentTime.TimeOfDay:hh\\:mm} - {closeTime.TimeOfDay:hh\\:mm}");
                    }

                    response.AvailableSlots[court.CourtName] = availableTimes;
                }

                return Result<AvailableSlotDto.AvailableSlotsResponse>.Success(response);
            }

            private List<Booking> ExpandRecurringBookings(List<Booking> bookings, DateTime date, DateTime openTime, DateTime closeTime)
            {
                var expandedBookings = new List<Booking>();

                foreach (var booking in bookings)
                {
                    if (string.IsNullOrEmpty(booking.RecurrenceRule))
                    {
                        // If no recurrence, just add the booking
                        expandedBookings.Add(booking);
                    }
                    else
                    {
                        // Parse the recurrence rule
                        var ruleParts = booking.RecurrenceRule.Split(';');
                        var freq = ruleParts.FirstOrDefault(r => r.StartsWith("FREQ="))?.Split('=')[1];
                        var untilStr = ruleParts.FirstOrDefault(r => r.StartsWith("UNTIL="))?.Split('=')[1];
                        
                        if (freq == "DAILY" && DateTime.TryParseExact(untilStr, "yyyyMMdd'T'HHmmss'Z'", null, System.Globalization.DateTimeStyles.AssumeUniversal, out var untilDate))
                        {
                            var occurrenceDate = booking.StartTime.Date;

                            while (occurrenceDate <= untilDate.Date && occurrenceDate <= date.Date)
                            {
                                // Add only the occurrences on the requested date
                                if (occurrenceDate == date.Date)
                                {
                                    var occurrenceStart = occurrenceDate.Add(booking.StartTime.TimeOfDay);
                                    var occurrenceEnd = occurrenceDate.Add(booking.EndTime.TimeOfDay);

                                    if (occurrenceStart < closeTime && occurrenceEnd > openTime)
                                    {
                                        // Adjust the occurrence to be within open and close times
                                        occurrenceStart = occurrenceStart < openTime ? openTime : occurrenceStart;
                                        occurrenceEnd = occurrenceEnd > closeTime ? closeTime : occurrenceEnd;

                                        expandedBookings.Add(new Booking
                                        {
                                            StartTime = occurrenceStart,
                                            EndTime = occurrenceEnd,
                                            CourtId = booking.Court.Id,
                                        });
                                    }
                                }
                                occurrenceDate = occurrenceDate.AddDays(1);
                            }
                        }
                    }
                }

                return expandedBookings.OrderBy(b => b.StartTime).ToList();
            }
        }
    }
}