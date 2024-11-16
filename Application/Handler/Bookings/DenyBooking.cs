using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class DenyBooking
    {
         public class Command : IRequest<Result<BookingDtoV2>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BookingDtoV2>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<BookingDtoV2>> Handle(Command request, CancellationToken cancellationToken)
            {
                var booking = await _context.Bookings
                .Include(b=>b.Payment)
                .Include(b=>b.Court)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (booking == null)
                {
                    return Result<BookingDtoV2>.Failure("Booking không được tìm thấy.");
                }
                if(booking.IsSuccess || booking.Status != BookingStatus.Pending){
                    return Result<BookingDtoV2>.Failure("Booking đã được xác nhận/từ chối trước đó.");
                }
                booking.Status = BookingStatus.Declined;
                _context.Bookings.Update(booking);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<BookingDtoV2>.Failure("Từ chối lịch lịch thất bại.");
                return Result<BookingDtoV2>.Success(_mapper.Map<BookingDtoV2>(_context.Entry(booking).Entity));
            }
        }
    }
}