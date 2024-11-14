using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
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

        public class Handler(DataContext _context, IMapper _mapper) : IRequestHandler<Query, Result<BookingDetailsDto>>
        {

            public async Task<Result<BookingDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var booking = await _context.Bookings.ProjectTo<BookingDtoV2ForDetails>(_mapper.ConfigurationProvider)
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