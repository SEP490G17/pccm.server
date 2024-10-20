using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams.BannerSpec;
using Application.SpecParams;
using AutoMapper;
using Domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Application.SpecParams.UserSpecification;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Users
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<AppUser>>>
        {
            public int pageIndex { get; set; }
            public int pageSize { get; set; }
            public string searchString { get; set; }
        }
        public class Handler(DataContext _context, IUserAccessor _userAccessor) : IRequestHandler<Query, Result<Pagination<AppUser>>>
        {
            public async Task<Result<Pagination<AppUser>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var data = await _userAccessor.GetUsers(request.pageIndex, request.pageSize, request.searchString);
                var count = await _context.Users.CountAsync();
                var result = new Pagination<AppUser>(request.pageSize, count, data);
                return Result<Pagination<AppUser>>.Success(result);
            }
        }
    }
}
