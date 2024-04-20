using ArbiWriter.Data;
using ArbiWriter.Models;
using ArbiWriter.Services.Interfaces;
using ccxt;
using Microsoft.EntityFrameworkCore;

namespace ArbiWriter.Services.Impl
{
    public class ExchangeService(IRepository<ExchangeEntity, string> exchangeRepository) : IExchangeService
    {
        public static readonly ICollection<Exchange> ExchangeObjects =
            [
                new Binance(),
                new Mexc(),
                new Kucoin(),
                new Kraken(),
                new Bybit(),
                new Huobi(),
                new Gateio(),
                new Okx(),
                new Bitmex(),
                new Bitget(),
            ];

        protected readonly IRepository<ExchangeEntity, string> _exchangeRepo = exchangeRepository;

        public ExchangeEntity CreateExchangeEntity(Exchange exchange) => new()
        {
            Id = exchange.id,
            Name = exchange.name as string ?? "",
            Url = exchange.url,
            Working = true
        };

        public ICollection<Exchange> GetSupportedExchanges() => ExchangeService.ExchangeObjects;

        public async Task MarkAllAsWorkingAsync(bool bWorking = true, CancellationToken stoppingToken = default)
        {
            // Mark each exchange as "Working = false"
            foreach (var x in _exchangeRepo.GetDbSet())
            {
                x.Working = bWorking;
                _exchangeRepo.Update(x, stoppingToken);
            }
            await _exchangeRepo.SaveChangesAsync(stoppingToken);
        }

        public async Task UploadData(CancellationToken stoppingToken = default)
        {
            // Get all supported exchanges
            List<Exchange> _exchangeObjects = GetSupportedExchanges().ToList();
            foreach (Exchange _exchangeObject in _exchangeObjects)
            {
                // Try to find exchange
                ExchangeEntity? ex = await _exchangeRepo.GetById(_exchangeObject.id, stoppingToken);

                // Mark as "Working = true" if found
                if (ex is not null)
                {
                    ex.Working = true;
                    _exchangeRepo.Update(ex);
                }
                else
                {
                    // Create new entity
                    ExchangeEntity entity = CreateExchangeEntity(_exchangeObject);
                    await _exchangeRepo.Add(entity, stoppingToken);
                }
            }

            await _exchangeRepo.SaveChangesAsync(stoppingToken);
        }
    }
}
