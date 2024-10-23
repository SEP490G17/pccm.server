using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using AutoMapper;
using MediatR;
using Domain;
using Application.SpecParams.UserSpecification;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace Application.Handler.Users
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<UserDto>>>
        {
            public BaseSpecParam BaseSpecParam { get; set; }
        }
        public class Handler(IUnitOfWork _unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<Pagination<UserDto>>>
        {
            public async Task<Result<Pagination<UserDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = request.BaseSpecParam;
                var spec = new UsersSpecification(query);
                var specCount = new UsersCountSpecification(query);
                var totalCount = await _unitOfWork.Repository<AppUser>().CountAsync(specCount,cancellationToken);
                var users = await _unitOfWork.Repository<AppUser>().QueryList(spec)
                .ProjectTo<UserDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                var result = new Pagination<UserDto>(query.PageSize, totalCount, users);
                return Result<Pagination<UserDto>>.Success(result);
            }
        }
    }
}
