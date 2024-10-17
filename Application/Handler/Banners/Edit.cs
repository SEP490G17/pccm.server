using Application.Core;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Handler.Banners
{
    public class Edit
    {
        public class Command : IRequest<Result<Banner>>
        {
            public Banner Banner { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Banner).SetValidator(new BannerValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<Banner>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Banner>> Handle(Command request, CancellationToken cancellationToken)
            {
                var banner = await _context.Banners.FindAsync(request.Banner.Id);
                _mapper.Map(request.Banner, banner);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Banner>.Failure("Faild to edit banner");
                return Result<Banner>.Success(_context.Entry(request.Banner).Entity);
            }
        }
    }
}
