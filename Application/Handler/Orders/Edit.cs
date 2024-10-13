using Application.Core;
using Application.DTOs;
using Application.Handler.Courts;
using AutoMapper;
using Domain;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Orders
{
    public class Edit
    {
        public class Command : IRequest<Result<Order>>
        {
            public OrderInputDTO order { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.order).SetValidator(new OrderValidator());

            }
        }
        public class Handler : IRequestHandler<Command, Result<Order>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Order>> Handle(Command request, CancellationToken cancellationToken)
            {
                var order = _mapper.Map<Order>(request.order);
                var existingOrder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == order.Id);
                if (existingOrder == null)
                {
                    return Result<Order>.Failure("Not found order.");
                }

                _context.Entry(existingOrder).State = EntityState.Detached;
                _context.Orders.Update(order);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Order>.Failure("Edited failed order.");
                return Result<Order>.Success(_context.Entry(order).Entity);
            }
        }
    }
}