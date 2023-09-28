using Microsoft.EntityFrameworkCore;

namespace StarCellar.Api.Data
{
    internal class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Wine> Wines { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasMany(u => u.Wines).WithOne(t => t.Owner);
            builder.Entity<Wine>().HasOne(t => t.Owner).WithMany(u => u.Wines);
        }
    }
}