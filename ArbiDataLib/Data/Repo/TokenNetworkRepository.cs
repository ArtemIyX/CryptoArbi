using ArbiDataLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ArbiDataLib.Data.Repo
{
    public class TokenNetworkRepository(ArbiDbContext context) : IRepository<ExchangeTokenNetwork, long>
    {
        private readonly ArbiDbContext _context = context;

        public DbSet<ExchangeTokenNetwork> GetDbSet() => _context.TokenNetworks;

        public async Task SaveChangesAsync(CancellationToken stoppingToken = default)
            => await _context.SaveChangesAsync(stoppingToken);

        public async Task Add(ExchangeTokenNetwork entity, CancellationToken stoppingToken = default)
            => await _context.TokenNetworks.AddAsync(entity, stoppingToken);

        public IEnumerable<ExchangeTokenNetwork> GetAll() => _context.TokenNetworks.AsEnumerable();

        public async Task<ExchangeTokenNetwork?> GetById(long id, CancellationToken stoppingToken = default)
            => await _context.TokenNetworks.FirstOrDefaultAsync(x => x.Id == id, stoppingToken);

        public void Update(ExchangeTokenNetwork entity, CancellationToken stoppingToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task Delete(long id, CancellationToken stoppingToken = default)
        {
            ExchangeTokenNetwork? entity = await GetById(id, stoppingToken);
            if (entity is not null)
            {
                _context.TokenNetworks.Remove(entity);
            }
        }

        public IQueryable<ExchangeTokenNetwork> AsQueryable() => _context.TokenNetworks.AsQueryable();

        public async Task AddRange(IEnumerable<ExchangeTokenNetwork> entities, CancellationToken stoppingToken = default)
            => await _context.TokenNetworks.AddRangeAsync(entities, stoppingToken);

        public void Delete(ExchangeTokenNetwork entity, CancellationToken stoppingToken = default)
            => _context.TokenNetworks.Remove(entity);

        public void DeleteRange(IEnumerable<ExchangeTokenNetwork> enttities, CancellationToken stoppingToken = default)
            => _context.TokenNetworks.RemoveRange(enttities);
    }
}
