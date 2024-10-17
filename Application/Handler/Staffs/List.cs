using Application.Core;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Staffs
{
    public class List
    {
        public class Query : IRequest<Result<List<StaffDetail>>> { }

        public class Handler : IRequestHandler<Query, Result<List<StaffDetail>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<List<StaffDetail>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var staffs = await _context.StaffDetails.Include(a => a.User).ToListAsync(cancellationToken);
                return Result<List<StaffDetail>>.Success(staffs);
            }
        }

    }
}