using System.Text.Json;
using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
              Email = "adminstrator@test.com",
              ImageUrl = ""
            },
            new AppUser
            {
              FirstName="Chủ cụm sân",
              UserName="courtOwner",
              LastName="1",
              Email="courtOwner@test.com",
              ImageUrl = ""
            }

          };

        foreach (var user in users)
        {
          await userManager.CreateAsync(user, "123456aA@");
        }
      }



      // if (!context.Banners.Any())
      // {
      //   var bannersData = File.ReadAllText("../Persistence/SeedData/banners.json");
      //   var banners = JsonSerializer.Deserialize<List<Banner>>(bannersData);
      //   context.Banners.AddRange(banners);

      // }

      // if (!context.NewsBlogs.Any())
      // {
      //   var newsData = File.ReadAllText("../Persistence/SeedData/news.json");
      //   var newsBlogs = JsonSerializer.Deserialize<List<NewsBlog>>(newsData);
      //   context.NewsBlogs.AddRange(newsBlogs);
      // }

      var rolesData = File.ReadAllText("../Persistence/SeedData/roles.json");
      var roles = JsonSerializer.Deserialize<List<IdentityRole>>(rolesData);
      if (context.Roles.Count() == 0)
      {
        var dbRoles = context.Roles.ToList();
        var roleName = new List<string>();
        dbRoles.ForEach(role => roleName.Add(role.Name));
        foreach (var role in roles)
        {
          if (!roleName.Contains(role.Name))
          {
            await roleManager.CreateAsync(role);
          }
        }
      }

      if (context.StaffPositions.Count() == 0)
      {
        var managerBanner = "ManagerBanner";
        var managerNews = "ManagerNews";
        var managerStaff = "ManagerStaff";
        var managerCourtCluster = "ManagerCourtCluster";
        var managerBooking = "ManagerBooking";
        var managerCustomer = "ManagerCustomer";


        var managerPosition = new StaffPosition()
        {
          Name = "Quản lý",
          DefaultRoles = [managerBanner, managerNews, managerStaff, managerCourtCluster, managerBooking]
        };

        var staffOne = new StaffPosition()
        {
          Name = "Nhân viên tiếp tân",
          DefaultRoles = [managerBooking, managerCustomer]
        };

        var staffTwo = new StaffPosition()
        {
          Name = "Nhân viên truyền thông",
          DefaultRoles = [managerBanner, managerNews]
        };

        await context.StaffPositions.AddRangeAsync(managerPosition, staffOne, staffTwo);
        await context.SaveChangesAsync();

      }

      // if (!context.Categories.Any())
      // {
      //   var categoriesData = File.ReadAllText("../Persistence/SeedData/categories.json");
      //   var categories = JsonSerializer.Deserialize<List<Category>>(categoriesData);
      //   await context.Categories.AddRangeAsync(categories);
      // }

      // if (!context.CourtClusters.Any())
      // {
      //   var courtClustersData = File.ReadAllText("../Persistence/SeedData/courtCluster.json");
      //   var CourtClusters = JsonSerializer.Deserialize<List<CourtCluster>>(courtClustersData);
      //   var adminId = await userManager.FindByNameAsync("adminstrator");

      //   CourtClusters.ForEach(c =>
      //   {
      //     var servicesData = File.ReadAllText("../Persistence/SeedData/services.json");
      //     var services = JsonSerializer.Deserialize<List<Service>>(servicesData);
      //     var productsData = File.ReadAllText("../Persistence/SeedData/products.json");
      //     var products = JsonSerializer.Deserialize<List<Product>>(productsData);

      //     c.OwnerId = adminId.Id;
      //     var courtsData = File.ReadAllText("../Persistence/SeedData/courts.json");
      //     var courts = JsonSerializer.Deserialize<List<Court>>(courtsData);
      //     courts.ForEach(c =>
      //     {
      //       var courtPrice = File.ReadAllText("../Persistence/SeedData/courtPrice.json");
      //       var courtPrices = JsonSerializer.Deserialize<List<CourtPrice>>(courtPrice);
      //       c.CourtPrices = courtPrices;
      //     });
      //     c.Courts = courts.ToList();
      //     c.Services = services;
      //     c.Products = products;
      //   });
      //   await context.CourtClusters.AddRangeAsync(CourtClusters);
      // }



      if (context.StaffDetails.Count() == 0)
      {
        var users = new List<AppUser>();
        var staffPositions = await context.StaffPositions.ToListAsync();
        var quanly = staffPositions.FirstOrDefault(s => s.Name.Equals("Quản lý"));
        var tieptan = staffPositions.FirstOrDefault(s => s.Name.Equals("Nhân viên tiếp tân"));
        var truyenthong = staffPositions.FirstOrDefault(s => s.Name.Equals("Nhân viên truyền thông"));

        for (int i = 1; i <= 10; i++)
        {
          var user = new AppUser()
          {
            FirstName = $"Staff",
            LastName = $" {i}",
            UserName = $"staff{i}",
            Email = $"staff{i}@test.com"
          };
          users.Add(user);
        }
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        var results = userManager.Users.Where(u => u.UserName.Contains("staff")).ToList();
        var staffs = new List<StaffDetail>(){
          new StaffDetail()
            {
              Position = quanly,
              UserId = results[0].Id,
            }
        };
        await context.AddRangeAsync(staffs);
        await context.SaveChangesAsync();
        await userManager.AddToRolesAsync(results[0], quanly.DefaultRoles);
        for (int i = 1; i < results.Count(); i++)
        {
          var user = results[i];
          var isTiepTan = i % 2 == 0;
          staffs.Add(new StaffDetail()
          {
            UserId = user.Id,
            Position = isTiepTan ? tieptan : truyenthong,
            Salary = i * 10000
          });
          await userManager.AddToRolesAsync(user, isTiepTan ? tieptan.DefaultRoles : truyenthong.DefaultRoles);
        }

        await context.StaffDetails.AddRangeAsync(staffs);


      }
      await context.SaveChangesAsync();
    }
  }
}