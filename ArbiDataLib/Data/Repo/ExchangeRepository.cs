using ArbiDataLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ArbiDataLib.Data.Repo
{
    public class ExchangeRepository(ArbiDbContext context) : IRepository<ExchangeEntity, string>
    {
        private readonly ArbiDbContext _context = context;

        public DbSet<ExchangeEntity> GetDbSet() => _context.Exchanges;

        public async Task SaveChangesAsync(CancellationToken stoppingToken = default)
            => await _context.SaveChangesAsync(stoppingToken);

        public async Task Add(ExchangeEntity entity, CancellationToken stoppingToken = default)
            => await _context.Exchanges.AddAsync(entity, stoppingToken);

        public IEnumerable<ExchangeEntity> GetAll() => _context.Exchanges.AsEnumerable();

        public async Task<ExchangeEntity?> GetById(string id, CancellationToken stoppingToken = default)
            => await _context.Exchanges.FirstOrDefaultAsync(x => x.Id == id, stoppingToken);

        public void Update(ExchangeEntity entity, CancellationToken stoppingToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified; 
        }

        public async Task Delete(string id, CancellationToken stoppingToken = default)
        {
            ExchangeEntity? entity = await GetById(id, stoppingToken);
            if (entity is not null)
            {
                _context.Exchanges.Remove(entity);
            }
        }

        public IQueryable<ExchangeEntity> AsQueryable() => _context.Exchanges.AsQueryable();
    }
}
