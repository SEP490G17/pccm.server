using Application.Core;
using Application.DTOs;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain;
using MediatR;
using Persistence;

namespace Application.Handler.Users
{
    public class EditProfile
    {
        public class Command : IRequest<Result<ProfileInputDto>>
        {
            public AppUser User { get; set; }
            public ProfileInputDto ProfileInputDto { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<ProfileInputDto>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<ProfileInputDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _context.Users.FindAsync(request.User.Id);
                    if (user == null) return Result<ProfileInputDto>.Failure("User not found");

                    _mapper.Map(request.ProfileInputDto, user);

                    var result = await _context.SaveChangesAsync() > 0;

                    if (!result) return Result<ProfileInputDto>.Failure("Failed to edit profile");

                    return Result<ProfileInputDto>.Success(request.ProfileInputDto);
                }
                catch (Exception ex)
                {
                    return Result<ProfileInputDto>.Failure(ex.Message);
                }
            }
        }
    }
}
