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


      if (!context.StaffDetails.Any())  // Use Any() for better performance
      {
        var users = new List<AppUser>();
        var staffPositions = await context.StaffPositions.ToListAsync();

        // Find positions more efficiently by using a dictionary for lookup
        var positionLookup = staffPositions.ToDictionary(p => p.Name, p => p);

        if (!positionLookup.TryGetValue("Quản lý", out var quanly) ||
            !positionLookup.TryGetValue("Nhân viên tiếp tân", out var tieptan) ||
            !positionLookup.TryGetValue("Nhân viên truyền thông", out var truyenthong))
        {
          throw new InvalidOperationException("One or more staff positions are missing from the database.");
        }

        // Create and add users in bulk
        for (int i = 1; i <= 10; i++)
        {
          var user = new AppUser
          {
            FirstName = $"Staff",
            LastName = $" {i}",
            UserName = $"staff{i}",
            Email = $"staff{i}@test.com"
          };
          users.Add(user);
        }

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();  // Save changes after user creation

        // Retrieve the users created in the previous step
        var results = await userManager.Users.Where(u => u.UserName.Contains("staff")).ToListAsync();

        var staffs = new List<StaffDetail>
                {
                    new StaffDetail
                    {
                        Position = quanly,
                        UserId = results[0].Id,
                    }
                };
        // Add the first staff member with the 'Quản lý' position
        await context.StaffDetails.AddAsync(staffs[0]);
        await context.SaveChangesAsync();
        staffs.Clear();
        for (int i = 1; i < results.Count; i++)
        {
          var user = results[i];
          var isTiepTan = i % 2 == 0;  // Alternate between positions
          var position = isTiepTan ? tieptan : truyenthong;
          staffs.Add(new StaffDetail
          {
            UserId = user.Id,
            Position = position,
          });
          await userManager.AddToRolesAsync(user, position.DefaultRoles);
        }
        // Add remaining staff details to the context
        await context.StaffDetails.AddRangeAsync(staffs);
      }
      await context.SaveChangesAsync();
    }
  }
}