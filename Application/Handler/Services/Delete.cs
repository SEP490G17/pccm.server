using Application.Core;
using MediatR;
using Persistence;

namespace Application.Handler.Services
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
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
                var service = await _context.Services.FindAsync(request.Id);
                if (service is null) return null;
                service.DeletedAt = DateTime.Now;
                service.DeletedBy = "Anonymous";
                _context.Update(service);
                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to delete the service.");
            }
        }
    }
}
