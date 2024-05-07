using ArbiDataLib.Data;
using ArbiDataLib.Models;
using ccxt;
using Microsoft.EntityFrameworkCore;
using Nethereum.Util;

namespace ArbiWriter.Services
{
    public interface ITokenCollector
    {
        public Task PrepareExchanges(CancellationToken stoppingToken = default);
    }

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
            await Task.Delay(1000, stoppingToken);
            List<Exchange> exchanges = [];
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IExchangeService exchangeService =
                   scope.ServiceProvider.GetRequiredService<IExchangeService>();
                exchanges = [.. exchangeService.GetSupportedExchanges()];
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWorkAsync(exchanges, stoppingToken);
            }
        }

        private async Task DoWorkAsync(List<Exchange> exchanges, CancellationToken stoppingToken = default)
        {
            List<Task> tasks = [];

            foreach (var exchange in exchanges)
            {
                tasks.Add(RunExchangeAsync(exchange, stoppingToken));
            }

            // Wait until stoppingToken.IsCancellationRequested
            await Task.WhenAll(tasks);
        }

        private async Task RunExchangeAsync(Exchange exchange, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await HandleExchangeAsync(exchange, stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task HandleExchangeAsync(Exchange exchange, CancellationToken stoppingToken = default)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                ITokenService tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
                try
                {
                    await tokenService.UpdateTokens(exchange, stoppingToken);
                    await tokenService.UpdateNetworks(exchange, stoppingToken);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    _logger.LogInformation($"{exchange.id} fetching finished");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{exchange.id} fetching error: {ex.Message}");
                }
            }
        }

        public async Task PrepareExchanges(CancellationToken stoppingToken = default)
        {
            _logger.LogInformation($"Preparing exchanges...");
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IExchangeService exchangeService =
                    scope.ServiceProvider.GetRequiredService<IExchangeService>();
                // Stop all work
                await exchangeService.MarkAllAsWorkingAsync(false, stoppingToken);
                // Mark working exchanges
                await exchangeService.UploadData(stoppingToken);
            }
            _logger.LogInformation($"Preparing exchanges finished");
        }
    }
}
