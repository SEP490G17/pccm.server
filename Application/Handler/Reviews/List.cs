using Application.Core;
using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Reviews
{
    public class List
    {
        public class Query : IRequest<Result<List<Review>>> { }
        public class Handler : IRequestHandler<Query, Result<List<Review>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<List<Review>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var reviewGroup = await _context.Reviews
                .ToListAsync(cancellationToken);

                return Result<List<Review>>.Success(reviewGroup);
            }
        }
    }
}
