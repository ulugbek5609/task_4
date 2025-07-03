using Microsoft.EntityFrameworkCore;

namespace UserManagementApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // ✅ Email UNIQUE index

            base.OnModelCreating(modelBuilder);
        }
    }
}