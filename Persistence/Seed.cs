using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
  public class Seed
  {
    public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
    {
      if (!userManager.Users.Any())
      {
        var users = new List<AppUser>
          {
            new AppUser
            {
              FirstName = "Alexandros",
              LastName = "Papadopoulos",
              UserName = "adminstrator",
              Email = "adminstrator@test.com"
            },

          };
        foreach (var user in users)
        {
          await userManager.CreateAsync(user, "123456aA@");
        }
      }
      await context.SaveChangesAsync();
    }
  }
}