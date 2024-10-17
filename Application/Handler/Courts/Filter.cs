using Application.Core;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Courts
{
    public class Filter
    {
        public class Query : IRequest<Result<List<Court>>>
        {
            public string value { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Court>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<Court>>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (Enum.TryParse(typeof(CourtStatus), request.value, true, out var statusEnum))
                {
                    var status = (CourtStatus)statusEnum;

                    var courts = await _context.Courts
                        .Where(x => x.Status == status)
                        .ToListAsync(cancellationToken);

                    return Result<List<Court>>.Success(courts);
                }
                else
                {
                    // Nếu không thể chuyển đổi, trả về lỗi hoặc một danh sách trống
                    return Result<List<Court>>.Failure("Not found");
                }
            }
        }
    }
}