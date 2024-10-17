using System.Text.Json;
using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
  public class Seed
  {
    public static async Task SeedData(DataContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
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

      if (!context.NewsBlogs.Any())
      {
        var newsData = File.ReadAllText("../Persistence/SeedData/news.json");
        var newsBlogs = JsonSerializer.Deserialize<List<NewsBlog>>(newsData);
        context.NewsBlogs.AddRange(newsBlogs);
      }

      if (!context.Roles.Any())
      {
        var rolesData = File.ReadAllText("../Persistence/SeedData/roles.json");
        var roles = JsonSerializer.Deserialize<List<IdentityRole>>(rolesData);
        foreach (var role in roles)
        {
          await roleManager.CreateAsync(role);
        }
      }
      if (!context.CourtClusters.Any())
      {
        var courtClustersData = File.ReadAllText("../Persistence/SeedData/courtCluster.json");
        var CourtClusters = JsonSerializer.Deserialize<List<CourtCluster>>(courtClustersData);
        await context.CourtClusters.AddRangeAsync(CourtClusters);
      }
      if (!context.Services.Any())
      {
        var servicesData = File.ReadAllText("../Persistence/SeedData/services.json");
        var services = JsonSerializer.Deserialize<List<Service>>(servicesData);
        await context.Services.AddRangeAsync(services);
      }
      await context.SaveChangesAsync();
    }
  }
}