using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Bookings
{
    public class DenyBookingConflict
    {
        public class Command : IRequest<Result<List<BookingDtoV2>>>
        {
            public List<int> Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<List<BookingDtoV2>>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<List<BookingDtoV2>>> Handle(Command request, CancellationToken cancellationToken)
            {
                var bookings = await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Court)
                .ThenInclude(b => b.CourtCluster)
                .Where(x => request.Id.Contains(x.Id) && !x.IsSuccess && x.Status == BookingStatus.Pending)
                .ToListAsync(cancellationToken);

                if (bookings == null || !bookings.Any())
                {
                    return Result<List<BookingDtoV2>>.Failure("Không có booking nào cần xóa hoặc đã được xác nhận/từ chối trước đó.");
                }

                foreach (var booking in bookings)
                {
                    booking.Status = BookingStatus.Declined;
                }

                _context.Bookings.UpdateRange(bookings); // Cập nhật tất cả các booking
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<List<BookingDtoV2>>.Failure("Xóa lịch thất bại.");
                }

                var bookingDtos = _mapper.Map<List<BookingDtoV2>>(bookings);
                return Result<List<BookingDtoV2>>.Success(bookingDtos);
            }
        }
    }
}