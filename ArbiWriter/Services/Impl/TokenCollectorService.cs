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

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await TurnOff(stoppingToken); 
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken = default)
        {
            //using IServiceScope scope = serviceScopeFactory.CreateScope();
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

        public async Task TurnOff(CancellationToken stoppingToken = default)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            IExchangeService exchangeService =
                scope.ServiceProvider.GetRequiredService<IExchangeService>();
            // Stop all work
            await exchangeService.MarkAllAsWorkingAsync(false, stoppingToken);
        }
    }
}
