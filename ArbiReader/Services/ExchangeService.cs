using ArbiDataLib.Data.Data.Repo;
using ArbiDataLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ArbiReader.Services
{
    public class ExchangeService(IRepository<ExchangeEntity, string> exchangeRepository) : IExchangeService
    {
        private readonly IRepository<ExchangeEntity, string> _exchangeRepo = exchangeRepository;

        
        public async Task<IList<ExchangeEntityResponse>> Get() => 
            await _exchangeRepo.AsQueryable().Select(x => x.ToReposnse()).ToListAsync();
        public async Task<IList<ExchangeEntityResponse>> Working() => 
            await _exchangeRepo.AsQueryable().Where(x => x.Working).Select(x => x.ToReposnse()).ToListAsync();
    }
}
