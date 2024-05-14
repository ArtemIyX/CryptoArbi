using ArbiDataLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbiDataLib.Data.Repo
{
    public class OrderBookRepository(ArbiDbContext context) : IRepository<OrderBookItem, long>
    {
        private readonly ArbiDbContext _context = context;

        public DbSet<OrderBookItem> GetDbSet() => _context.OrderBooks;

        public async Task SaveChangesAsync(CancellationToken stoppingToken = default)
            => await _context.SaveChangesAsync(stoppingToken);

        public async Task Add(OrderBookItem entity, CancellationToken stoppingToken = default)
            => await _context.OrderBooks.AddAsync(entity, stoppingToken);

        public IEnumerable<OrderBookItem> GetAll() => _context.OrderBooks.AsEnumerable();

        public async Task<OrderBookItem?> GetById(long id, CancellationToken stoppingToken = default)
            => await _context.OrderBooks.FirstOrDefaultAsync(x => x.Id == id, stoppingToken);

        public void Update(OrderBookItem entity, CancellationToken stoppingToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task Delete(long id, CancellationToken stoppingToken = default)
        {
            OrderBookItem? entity = await GetById(id, stoppingToken);
            if (entity is not null)
            {
                _context.OrderBooks.Remove(entity);
            }
        }

        public IQueryable<OrderBookItem> AsQueryable() => _context.OrderBooks.AsQueryable();
    }
}
