using Application.Core;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Banners
{
    public class Create
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
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Banner>> Handle(Command request, CancellationToken cancellationToken)
            {
                var banner = request.Banner;
                await _context.AddAsync(banner, cancellationToken);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Banner>.Failure("Fail to create banner");
                var newBanner = _context.Entry(banner).Entity;
                return Result<Banner>.Success(newBanner);
            }
        }
    }
}
