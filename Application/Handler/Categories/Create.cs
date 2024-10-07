using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Categories
{
    public class Create
    {
        public class Command : IRequest<Result<Category>>
        {
            public Category Category { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Category).SetValidator(new CategoryValidator());

            }
        }

        public class Handler(DataContext _context) : IRequestHandler<Command, Result<Category>>
        {
            public async Task<Result<Category>> Handle(Command request, CancellationToken cancellationToken)
            {
                var category = request.Category;
                await _context.AddAsync(category, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Category>.Failure("Fail to create category");
                var newCategory = _context.Entry(category).Entity;
                return Result<Category>.Success(newCategory);
            }
        }
    }
}