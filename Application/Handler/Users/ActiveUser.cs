﻿using Application.Core;
using Application.DTOs;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Users
{
    public class ActiveUser
    {
        public class Command : IRequest<Result<Unit>>
        {
            public ActiveDTO user { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.user).SetValidator(new ActiveValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userExist = await _context.Users.FindAsync(request.user.Id);
                userExist.LockoutEnabled = request.user.IsActive;
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit>.Failure("Faild to edit user");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
