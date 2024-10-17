using Application.Core;
using Application.DTOs;
using Application.Handler.Services;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Products
{
    public class Create
    {
        public class Command : IRequest<Result<Product>>
        {
            public ProductInputDTO product { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.product).SetValidator(new ProductValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<Product>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<Product>> Handle(Command request, CancellationToken cancellationToken)
            {
                var product = _mapper.Map<Product>(request.product);
                await _context.AddAsync(product, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Product>.Failure("Fail to create product");
                var newProduct = _context.Entry(product).Entity;
                return Result<Product>.Success(newProduct);
            }
        }
    }
}
