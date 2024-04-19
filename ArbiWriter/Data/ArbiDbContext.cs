

using ArbiWriter.Models;
using Microsoft.EntityFrameworkCore;

namespace ArbiWriter.Data
{
    public class ArbiDbContext : DbContext
    {
        public ArbiDbContext(DbContextOptions<ArbiDbContext> options) : base(options)
        {
        }

        public DbSet<TestModel> YourEntities { get; set; }
    }
}
