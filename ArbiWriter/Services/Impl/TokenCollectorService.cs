using ArbiWriter.Data;
using ArbiWriter.Models;
using ArbiWriter.Services.Interfaces;
using ccxt;
using Microsoft.EntityFrameworkCore;
using Nethereum.Util;

namespace ArbiWriter.Services.Impl
{
    public class TokenCollectorService(
        ILogger<TokenCollectorService> logger,
        IServiceScopeFactory serviceScopeFactory) : BackgroundService, ITokenCollector
    {
        private const string ClassName = nameof(TokenCollectorService);
        private readonly ILogger<TokenCollectorService> _logger = logger;
        private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await PrepareExchanges(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWorkAsync(stoppingToken);
            }
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken = default)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            IExchangeService exchangeService =
               scope.ServiceProvider.GetRequiredService<IExchangeService>();
            ITokenService tokenService =
                scope.ServiceProvider.GetRequiredService<ITokenService>();

            List<Exchange> exchanges = exchangeService.GetSupportedExchanges().ToList();
            foreach (Exchange ex in exchanges)
            {
                await tokenService.UpdateTokens(ex, stoppingToken);
            }
            await Task.Delay(5000);
        }

        public async Task PrepareExchanges(CancellationToken stoppingToken = default)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            IExchangeService exchangeService =
                scope.ServiceProvider.GetRequiredService<IExchangeService>();
            // Stop all work
            await exchangeService.MarkAllAsWorkingAsync(false, stoppingToken);
            // Mark working exchanges
            await exchangeService.UploadData(stoppingToken);
        }
    }
}
