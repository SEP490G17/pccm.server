using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class BookingDetailsV1
    {
        public class Query : IRequest<Result<BookingDetailsDto>>
        {
            public int Id { get; set; }
        }

        public class Handler(DataContext _context, IMapper _mapper, IUserAccessor _userAccessor, UserManager<AppUser> _userManager) : IRequestHandler<Query, Result<BookingDetailsDto>>
        {

            public async Task<Result<BookingDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var check = await _context.Bookings.Include(b=>b.AppUser).FirstOrDefaultAsync(x=>x.Id == request.Id, cancellationToken);
                if(check.AppUser != null){
                    var user = check.AppUser;
                    var currUser = await _userManager.FindByNameAsync(_userAccessor.GetUserName());
                    var adminRoles = new List<string>() { "ManagerCourtCluster", "Owner", "ManagerBooking" };
                    var roles = await _userManager.GetRolesAsync(currUser);
                    var isAdmin = roles.Any(x=>adminRoles.Contains(x));
                    if(!isAdmin){
                        if(!currUser.UserName.Equals(user.UserName)){
                            return Result<BookingDetailsDto>.Failure("not found");
                        }
                    }

                }
                var booking = await _context.Bookings.ProjectTo<BookingDtoV2ForDetails>(_mapper.ConfigurationProvider, cancellationToken)
                                    .FirstOrDefaultAsync(x => x.Id == request.Id);



                var orders = await _context.Orders.Where(x => x.BookingId == request.Id)
                                    .ProjectTo<OrderOfBookingDto>(_mapper.ConfigurationProvider)
                                    .ToListAsync();

                if (booking == null) return Result<BookingDetailsDto>.Failure("Booking not found");
                var response = new BookingDetailsDto()
                {
                    BookingDetails = booking,
                    OrdersOfBooking = orders
                };
                return Result<BookingDetailsDto>.Success(response);
            }
        }
    }
}