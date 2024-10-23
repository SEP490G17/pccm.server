using Application.Core;
using Domain.Enum;
using MediatR;
using Persistence;


namespace Application.Handler.News
{
    public class ChangeStatus
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
            public int status { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {

            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var news = await _context.NewsBlogs.FindAsync(request.Id);
                if (request.status == 1)
                {
                    news.Status = BannerStatus.Display;
                }
                else
                {
                    news.Status = BannerStatus.Hidden;
                }
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit>.Failure("Faild to change status news");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
