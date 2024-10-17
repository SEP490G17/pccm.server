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
    public class Edit
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
                _context = context;
            }
            public async Task<Result<Product>> Handle(Command request, CancellationToken cancellationToken)
            {
                var product = _mapper.Map<Product>(request.product);
                var productExist = await _context.Products.FindAsync(request.product.Id);
                _mapper.Map(product, productExist);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Product>.Failure("Faild to edit product");
                return Result<Product>.Success(_context.Entry(product).Entity);
            }
        }
    }
}
