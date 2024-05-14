
using ArbiDataLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ArbiDataLib.Data
{
    public class ArbiDbContext : DbContext
    {
        public DbSet<ExchangeEntity> Exchanges { get; set; }
        public DbSet<ExchangeToken> ExchangeTokens { get; set; }
        public DbSet<ExchangeTokenNetwork> TokenNetworks { get; set; }
        public DbSet<OrderBookItem> OrderBooks { get; set; }

        public ArbiDbContext(DbContextOptions<ArbiDbContext> options) : base(options)
        {
            Database.EnsureCreated();
            //Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           /* MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
            optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=ArbiDB;Uid=writer;Pwd=12345678", serverVersion);*/
            optionsBuilder.UseLazyLoadingProxies(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeToken>()
                .Property(b => b.Updated)
                .HasDefaultValueSql("current_timestamp(6)");

            modelBuilder.Entity<ExchangeToken>()
               .HasMany(e => e.Networks)
               .WithOne(n => n.Token)
               .HasForeignKey(n => n.ExchangeTokenId);

            modelBuilder.Entity<ExchangeToken>()
              .HasMany(e => e.OrderBook)
              .WithOne(n => n.ExchangeToken)
              .HasForeignKey(n => n.ExchangeTokenId);
        }
    }
}
