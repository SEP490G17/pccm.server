﻿using Application.Core;
using MediatR;
using Persistence;

namespace Application.Handler.Categories
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var category = await _context.Categories.FindAsync(request.Id);
                    if (category is null) return null;
                    _context.Remove(category);
                    var result = await _context.SaveChangesAsync() > 0;
                    if (result) return Result<Unit>.Success(Unit.Value);
                    return Result<Unit>.Failure("Failed to delete the category.");
                }
                catch (Exception ex)
                {
                    return Result<Unit>.Failure(ex.Message);
                }

            }
        }
    }
}
