using Microsoft.EntityFrameworkCore;
using TrustGate.Api.Models;

namespace TrustGate.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppContext> options) : base(options)
        {

        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Entitlement> Entitlements => Set<Entitlement>();
        public DbSet<AccessRequest> AccessRequests => Set<AccessRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Store the entitlement id list as CSV
            modelBuilder.Entity<User>()
                .Property(u => u.EntitlementsIds)
                .HasConversion(
                v => string.Join(',', v),
                v => v.Length == 0
                    ? new List<int>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                );

            // Seed an admin
            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@trustgate.local",
                    Password = "admin123",
                    Role = "Admin",
                    IsActive = true
                });

        }
    }
}
