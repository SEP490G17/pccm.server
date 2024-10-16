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
    public class List
    {
        public class Query : IRequest<Result<List<BookingDTO>>> { }

        public class Handler : IRequestHandler<Query, Result<List<BookingDTO>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<List<BookingDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var bookings = await _context.Bookings.Include(a => a.Court).Include(a => a.User).Include(a => a.Staff).ToListAsync(cancellationToken);
                var bookingDTOs = _mapper.Map<List<BookingDTO>>(bookings);
                return Result<List<BookingDTO>>.Success(bookingDTOs);
            }
        }

    }
}