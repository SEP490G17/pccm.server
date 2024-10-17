using System.Text.Json;
using Domain;
using Domain.Entity;
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

      if (!context.Banners.Any())
      {
        var bannersData = File.ReadAllText("../Persistence/SeedData/banners.json");
        var banners = JsonSerializer.Deserialize<List<Banner>>(bannersData);
        context.Banners.AddRange(banners);

      }

      if(!context.NewsBlogs.Any()){
        var newsData = File.ReadAllText("../Persistence/SeedData/news.json");
        var newsBlogs = JsonSerializer.Deserialize<List<NewsBlog>>(newsData);
        context.NewsBlogs.AddRange(newsBlogs);
      }

      await context.SaveChangesAsync();
    }
  }
}