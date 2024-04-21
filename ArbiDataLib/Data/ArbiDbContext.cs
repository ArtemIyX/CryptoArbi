
using ArbiDataLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ArbiDataLib.Data
{
    public class ArbiDbContext : DbContext
    {
        public DbSet<ExchangeEntity> Exchanges { get; set; }
        public DbSet<ExchangeToken> ExchangeTokens { get; set; }

        public ArbiDbContext(DbContextOptions<ArbiDbContext> options) : base(options)
        {
            Database.EnsureCreatedAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeToken>()
                .Property(b => b.Updated)
                .HasDefaultValueSql("current_timestamp(6)");
        }
    }
}
