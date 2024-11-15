using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Orders
{
    public class CancelOrderV1
    {
        public class Command : IRequest<Result<OrderOfBookingDto>>
        {
            public int Id { get; set; }

        }

        public class Handler : IRequestHandler<Command, Result<OrderOfBookingDto>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<OrderOfBookingDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var order = await _context.Orders.Include(o => o.Payment).FirstOrDefaultAsync(x => x.Id == request.Id);
                if (order == null)
                {
                    return Result<OrderOfBookingDto>.Failure("Không tìm thấy Order");
                }
                if (order.Payment.Status == PaymentStatus.Success)
                {
                    return Result<OrderOfBookingDto>.Failure("Order đã được thanh toán không thể cancel");
                }
                order.Payment.Status = PaymentStatus.Cancel;
                _context.Update(order);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result) return Result<OrderOfBookingDto>.Failure("Cancel Order thất bại");

                var res = await _context.Orders.ProjectTo<OrderOfBookingDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(o => o.Id == request.Id);
                return Result<OrderOfBookingDto>.Success(res);

            }
        }
    }
}