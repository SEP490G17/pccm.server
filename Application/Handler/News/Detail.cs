using Application.Core;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.News
{
    public class Detail
    {
          public class Query : IRequest<Result<NewsBlog>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<NewsBlog>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                this._context = context;
            }
            public async Task<Result<NewsBlog>> Handle(Query request, CancellationToken cancellationToken)
            {
                var events = await _context.NewsBlogs.FirstOrDefaultAsync(x => x.Id == request.Id);
                
                if (events is null)
                    return Result<NewsBlog>.Failure("Event not found");
                return Result<NewsBlog>.Success(events);
            }
        }
    }
}