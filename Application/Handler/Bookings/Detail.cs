using Application.Core;
using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class Detail
    {
        public class Query : IRequest<Result<BookingDto>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<BookingDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }
            public async Task<Result<BookingDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var booking = await _context.Bookings
                .Include(a => a.Court)
                .Include(a=>a.AppUser)
                .Include(a=>a.Staff)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (booking == null) return Result<BookingDto>.Failure("Booking not found");
                var bookingDTOs = _mapper.Map<BookingDto>(booking);
                return Result<BookingDto>.Success(bookingDTOs);
            }
        }
    }
}