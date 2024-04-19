
using ArbiWriter.Models;
using Microsoft.EntityFrameworkCore;

namespace ArbiWriter.Data
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
            optionsBuilder.UseLazyLoadingProxies();
        }

    }
}
