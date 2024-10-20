using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Users
{
    public class Detail
    {
        public class Query : IRequest<Result<AppUser>>
        {
            public string Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<AppUser>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<AppUser>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(request.Id));
                if (user is null)
                    return Result<AppUser>.Failure("user not found");
                return Result<AppUser>.Success(user);
            }
        }
    }
}
