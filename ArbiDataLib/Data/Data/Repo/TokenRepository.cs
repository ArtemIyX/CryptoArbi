using ArbiDataLib.Data.Data.Repo;
using ArbiDataLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ArbiDataLib.Data.Repo
{
    public class TokenRepository(ArbiDbContext context) : IRepository<ExchangeToken, int>
    {
        private readonly ArbiDbContext _context = context;

        public DbSet<ExchangeToken> GetDbSet() => _context.ExchangeTokens;

        public async Task SaveChangesAsync(CancellationToken stoppingToken = default)
            => await _context.SaveChangesAsync();

        public async Task Add(ExchangeToken entity, CancellationToken stoppingToken = default)
            => await _context.ExchangeTokens.AddAsync(entity, stoppingToken);

        public IEnumerable<ExchangeToken> GetAll() => _context.ExchangeTokens.AsEnumerable();

        public async Task<ExchangeToken?> GetById(int id, CancellationToken stoppingToken = default)
            => await _context.ExchangeTokens.FirstOrDefaultAsync(x => x.Id == id, stoppingToken);

        public void Update(ExchangeToken entity, CancellationToken stoppingToken = default)
        {
            entity.Updated = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task Delete(int id, CancellationToken stoppingToken = default)
        {
            ExchangeToken? entity = await GetById(id, stoppingToken);
            if (entity is not null)
            {
                _context.ExchangeTokens.Remove(entity);
            }
        }

        public IQueryable<ExchangeToken> AsQueryable() => _context.ExchangeTokens.AsQueryable();
    }
}
