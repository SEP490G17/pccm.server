using Application.Core;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Staffs
{
    public class Detail
    {
        public class Query : IRequest<Result<StaffDto>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<StaffDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }
            public async Task<Result<StaffDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var staff = await _context.StaffDetails.Include(a => a.User).FirstOrDefaultAsync(x => x.Id == request.Id);

                if (staff == null) return Result<StaffDto>.Failure("Staff not found!");
                var staffDtoMap = _mapper.Map<StaffDto>(staff);
                return Result<StaffDto>.Success(staffDtoMap);
            }
        }
    }
}
