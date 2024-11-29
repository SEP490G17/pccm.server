
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace Application.Handler.Notifications
{
    public class CreateNotification
    {

        public class Command : IRequest<NotificationDto>
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public NotificationType Type { get; set; }
            public string Url { get; set; }
            public AppUser AppUser { get; set; }
        }


        public class Handler : IRequestHandler<Command, NotificationDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<NotificationDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var notification = new Notification()
                {
                    Message = request.Message,
                    Title = request.Title,
                    Type = request.Type,
                    Url = request.Url,
                    AppUser = request.AppUser
                };

                await _context.AddAsync(notification, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                var result = _context.Entry(notification).Entity;
                return _mapper.Map<NotificationDto>(result);
            }
        }

    }
}