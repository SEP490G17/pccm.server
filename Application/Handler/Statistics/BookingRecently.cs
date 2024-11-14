using Application.Core;
using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Statistics
{
    public class BookingRecently
    {
        public class Query : IRequest<Result<List<BookingDtoStatistic>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<BookingDtoStatistic>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;


            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<BookingDtoStatistic>>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<BookingDtoStatistic> recent = new List<BookingDtoStatistic>();
                var data = await _context.Bookings
                .Include(b => b.AppUser)
                .Include(b => b.Court)
                .ThenInclude(b => b.CourtCluster)
                .Take(8)
                .ToListAsync();

                foreach (var item in data)
                {
                    var booking = _mapper.Map<BookingDtoStatistic>(item);
                    recent.Add(booking);
                }
                return Result<List<BookingDtoStatistic>>.Success(recent);
            }
        }
    }
}
