

using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.NotificationSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Notifications
{
    public class GetUserNoti
    {
        public class Query : IRequest<Result<PaginationNoti<NotificationDto>>>
        {
            public BaseSpecParam BaseSpecParam { get; set; }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IMapper _mapper) : IRequestHandler<Query, Result<PaginationNoti<NotificationDto>>>
        {
            public async Task<Result<PaginationNoti<NotificationDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecParam;
                var userName = _userAccessor.GetUserName();

                var spec = new NotificationsSpecification(querySpec, userName);
                var specCount = new NotificationCountSpecification(userName);
                var specUnreadCount = new NotificationCountUnreadSpecification(userName);

                var totalElement = await _unitOfWork.Repository<Notification>().CountAsync(specCount, cancellationToken);
                var totalUnread = await _unitOfWork.Repository<Notification>().CountAsync(specUnreadCount, cancellationToken);
                var data = await _unitOfWork.Repository<Notification>().QueryList(spec).ProjectTo<NotificationDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

                var response = new PaginationNoti<NotificationDto>()
                {
                    Count = totalElement,
                    Data = data,
                    PageSize = request.BaseSpecParam.PageSize,
                    NumOfUnread = totalUnread,
                };

                return Result<PaginationNoti<NotificationDto>>.Success(response);
            }
        }
    }
}