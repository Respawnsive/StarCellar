using Microsoft.EntityFrameworkCore;

namespace StarCellar.Api.Data
{
    internal class UserRefreshTokenDbContext : DbContext
    {
        public UserRefreshTokenDbContext(DbContextOptions<UserRefreshTokenDbContext> options)
            : base(options) { }

        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRefreshToken>().HasIndex(b => b.UserId);
        }
    }
}
