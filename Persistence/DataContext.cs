using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<CourtCluster> CourtClusters { get; set; }
        public DbSet<StaffDetail> StaffDetails { get; set; }
        public DbSet<StaffAssignment> StaffAssignments { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<NewsBlog> NewsBlogs { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<StaffPosition> StaffPositions { get; set; }
        public DbSet<CourtPrice> CourtPrices { get; set; }
        public DbSet<BannerLog> BannerLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CourtCluster>()
                .Property(c => c.Images)
                .HasColumnType("json"); // Đặt kiểu cột là JSON
            
            builder.Entity<NewsBlog>().ToTable("News")
                .Property(c => c.Tags)
                .HasColumnType("json"); // Đặt kiểu cột là JSON
                
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}