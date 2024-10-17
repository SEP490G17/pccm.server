using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Orders
{
    public class Create
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

        public class Handler(DataContext _context,IMapper _mapper) : IRequestHandler<Command, Result<Order>>
        {
          
            public async Task<Result<Order>> Handle(Command request, CancellationToken cancellationToken)
            {
                var order = _mapper.Map<Order>(request.order);
                await _context.AddAsync(order, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Order>.Failure("Order creation has failed.");
                var newOrder = _context.Entry(order).Entity;
                return Result<Order>.Success(newOrder);
            }
        }
    }
}