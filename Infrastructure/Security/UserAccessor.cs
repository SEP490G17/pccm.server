using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DataContext _context;
    public UserAccessor(IHttpContextAccessor httpContextAccessor, DataContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }
    public string GetUserName()
    {
        return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
    }

    public async Task<List<AppUser>> GetUsers(int pageIndex, int pageSize, string searchString)
    {
        var user = await _context.Users
            .OrderBy(b => b.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        if (!String.IsNullOrEmpty(searchString))
        {
            user = user.Where(b => b.UserName.Contains(searchString)).ToList();
        }
        return user;
    }

}
