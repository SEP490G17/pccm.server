using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Users
{
    public class Detail
    {
        public class Query : IRequest<Result<UserDto>>
        {
            public string username { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<UserDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<UserDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(request.username));
                if (user is null)
                    return Result<UserDto>.Failure("user not found");
                var res = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Success(res);
            }
        }
    }
}
