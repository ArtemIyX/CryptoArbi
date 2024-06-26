﻿using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ccxt;
using Microsoft.EntityFrameworkCore;

namespace ArbiWriter.Services
{
    public interface IExchangeService
    {
        public ICollection<Exchange> GetSupportedExchanges();
        public Task MarkAllAsWorkingAsync(bool bWorking = true, CancellationToken stoppingToken = default);
        public ExchangeEntity CreateExchangeEntity(Exchange exchange);
        public Task UploadData(CancellationToken stoppingToken = default);
    }

    public class ExchangeService(IRepository<ExchangeEntity, string> exchangeRepository) : IExchangeService
    {
        public static readonly ICollection<Exchange> ExchangeObjects =
            [
                //new Binance(),  - adress verification 
                new Mexc(),
                new Kucoin(),
                new Bybit(),
                new Huobi(),
                //new Okx(), - adress verification 
                new Bitget(),
                //new Gateio(), - no bid/ask volume
            ];

        protected readonly IRepository<ExchangeEntity, string> _exchangeRepo = exchangeRepository;

        public ExchangeEntity CreateExchangeEntity(Exchange exchange) => new()
        {
            Id = exchange.id,
            Name = exchange.name as string ?? "",
            Url = exchange.url,
            Working = true
        };

        public ICollection<Exchange> GetSupportedExchanges() => ExchangeObjects;

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
