using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Events
{
    public class Detail
    {
          public class Query : IRequest<Result<Event>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Event>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<Event>> Handle(Query request, CancellationToken cancellationToken)
            {
                var events = await _context.Events.FirstOrDefaultAsync(x => x.Id == request.Id);
                
                if (events is null)
                    return Result<Event>.Failure("Event not found");
                return Result<Event>.Success(events);
            }
        }
    }
}