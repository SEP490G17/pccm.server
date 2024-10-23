using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class RoleController(RoleManager<IdentityRole> _roleManager) : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetRoles(CancellationToken ct)
        {
            var roles = await _roleManager.Roles.ToListAsync(ct);
            string[] exceptRoles = ["Owner", "Admin"];
            List<string> roleName = new List<string>();
            foreach (var role in roles)
            {
                if (!exceptRoles.Contains(role.Name))
                {
                    roleName.Add(role.Name);
                }
            }
            return Ok(roleName);
        }
    }
}