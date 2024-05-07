﻿
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

        }
    }
}
