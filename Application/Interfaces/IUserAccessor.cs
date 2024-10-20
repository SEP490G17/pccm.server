using Application.DTOs;
using Domain;
using System.Numerics;

namespace Application.Interfaces;

public interface IUserAccessor
{
    string GetUserName();

    Task<List<AppUser>> GetUsers(int pageIndex, int pageSize, string searchString);
}
