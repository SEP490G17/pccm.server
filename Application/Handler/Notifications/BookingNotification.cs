using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Notifications
{
    public class BookingNotification
    {
        public class Command : IRequest<NotificationQueryResult>
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public NotificationType Type { get; set; }
            public string Url { get; set; }

            public int BookingId { get; set; }
        }


        public class Handler : IRequestHandler<Command, NotificationQueryResult>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<NotificationQueryResult> Handle(Command request, CancellationToken cancellationToken)
            {
                var booking = await _context.Bookings.Include(x => x.AppUser).FirstOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken);
                if (booking.AppUser == null)
                {
                    return null;
                }

                var notification = new Notification()
                {
                    Message = request.Message,
                    Title = request.Title,
                    Type = request.Type,
                    Url = request.Url,
                    AppUser = booking.AppUser
                };

                await _context.AddAsync(notification, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                var result = _context.Entry(notification).Entity;

                var response = new NotificationQueryResult()
                {
                    NotificationDto = _mapper.Map<NotificationDto>(result),
                    PhoneNumber = booking.AppUser.PhoneNumber,
                };
                return response;
            }
        }
    }
}