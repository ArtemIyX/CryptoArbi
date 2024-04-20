using Microsoft.EntityFrameworkCore;

namespace ArbiDataLib.Data.Repo
{
    public interface IRepository<T, G> where T : class
    {
        public IEnumerable<T> GetAll();
        public IQueryable<T> AsQueryable();
        public Task<T?> GetById(G id, CancellationToken stoppingToken = default);
        public Task Add(T entity, CancellationToken stoppingToken = default);
        public void Update(T entity, CancellationToken stoppingToken = default);
        public Task Delete(G id, CancellationToken stoppingToken = default);
        public DbSet<T> GetDbSet();
        public Task SaveChangesAsync(CancellationToken stoppingToken = default);
    }
}
