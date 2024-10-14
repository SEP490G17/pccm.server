using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Staffs
{
    public class Detail
    {
        public class Query : IRequest<Result<StaffDetail>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<StaffDetail>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }
            public async Task<Result<StaffDetail>> Handle(Query request, CancellationToken cancellationToken)
            {
                var staff = await _context.StaffDetails.Include(a => a.User).FirstOrDefaultAsync(x => x.Id == request.Id);

                if (staff == null) return Result<StaffDetail>.Failure("Staff not found!");
                return Result<StaffDetail>.Success(staff);
            }
        }
    }
}