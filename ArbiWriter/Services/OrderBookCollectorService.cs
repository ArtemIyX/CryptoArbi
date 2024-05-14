using ccxt;

namespace ArbiWriter.Services
{
    public interface IOrderBookCollector
    {
        public Task HandleExchangeAsync(Exchange exchange, CancellationToken stoppingToken = default);
    }

    public class OrderBookCollectorService(
        ILogger<OrderBookCollectorService> logger,
        IServiceScopeFactory serviceScopeFactory) : BackgroundService, IOrderBookCollector
    {
        private const string ClassName = nameof(TokenCollectorService);
        private readonly ILogger<OrderBookCollectorService> _logger = logger;
        private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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

        public async Task HandleExchangeAsync(Exchange exchange, CancellationToken stoppingToken = default)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                ITokenService tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
                try
                {
                    await tokenService.UpdateTokensOrderBook(exchange, stoppingToken);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    _logger.LogInformation($"{exchange.id} order book finished");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{exchange.id} order book error: {ex.Message}");
                }
            }
        }
    }
}
