using Application.Core;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Persistence;


namespace Application.Handler.News
{
    public class ChangeStatus
    {
        public class Command : IRequest<Result<NewsBlog>>
        {
            public int Id { get; set; }
            public int status { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<NewsBlog>>
        {

            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<NewsBlog>> Handle(Command request, CancellationToken cancellationToken)
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
                await _context.SaveChangesAsync();
               // if (!result) return Result<NewsBlog>.Failure("Faild to change status news");
                return Result<NewsBlog>.Success(news);
            }
        }
    }
}
