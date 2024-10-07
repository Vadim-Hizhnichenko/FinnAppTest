using Microsoft.EntityFrameworkCore;
using TestFinAppApi.Models;

namespace TestFinAppApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MarketAsset> MarketAssets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure MarketAsset entity
            modelBuilder.Entity<MarketAsset>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<MarketAsset>()
                .Property(m => m.Symbol)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<MarketAsset>()
                .Property(m => m.Price)
                .HasPrecision(18, 6);
        }
    }
}
