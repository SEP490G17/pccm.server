using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.BookingSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Bookings
{
    public class UserHistory
    {
        public class Query : IRequest<Result<Pagination<BookingUserHistoryDto>>>
        {
            public BookingUserHistorySpecParam BookingSpecParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork, IUserAccessor userAccessor, UserManager<AppUser> userManager) : IRequestHandler<Query, Result<Pagination<BookingUserHistoryDto>>>
        {
            public async Task<Result<Pagination<BookingUserHistoryDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await userManager.FindByNameAsync(userAccessor.GetUserName());
                if(user == null){
                return Result<Pagination<BookingUserHistoryDto>>.Failure("Username không tồn tại");
                }
                var querySpec = request.BookingSpecParam;
               Console.WriteLine(user.Id);
               querySpec.UserId = user.Id;
                var spec = new BookingUserHistorySpecification(querySpec);
                var specCount = new BookingUserHistorySpecification(querySpec);
                var data = await _unitOfWork.Repository<Booking>()
                    .QueryList(spec)
                    .ProjectTo<BookingUserHistoryDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                var total = await _unitOfWork.Repository<Booking>().CountAsync(specCount, cancellationToken);

                var response = new Pagination<BookingUserHistoryDto>()
                {
                    Count = total,
                    Data = data,
                    PageSize = request.BookingSpecParam.PageSize

                };
                return Result<Pagination<BookingUserHistoryDto>>.Success(response);
            }
        }
    }
}