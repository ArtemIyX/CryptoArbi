using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiLib.Libs;
using ccxt;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Diagnostics;

namespace ArbiWriter.Services
{
    public interface ITokenService
    {
        public Task UpdateTokensNetworks(Exchange owner, CancellationToken stoppingToken = default);
        public Task UpdateTokensOrderBook(Exchange owner, CancellationToken stoppingToken = default);
        public ExchangeToken? CreateTokenEntity(Exchange owner, in ccxt.Ticker ticker, in ccxt.Currency currency);
        public Task<ExchangeToken?> FindToken(string ownerId, string fullName, CancellationToken stoppingToken = default);
    }

    public class TokenService(IServiceScopeFactory serviceScopeFactory,
            ILogger<TokenService> logger) : ITokenService
    {
        private const string ClassName = nameof(TokenService);
        private readonly ILogger<TokenService> _logger = logger;
        private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

        public ExchangeToken? CreateTokenEntity(Exchange exchange, in ccxt.Ticker ticker, in ccxt.Currency currency)
        {
            string tickerName = TickerLib.RemoveSemiColon(ticker.symbol ?? "");

            // Must be X/USDT
            if (!TickerLib.IsUsdtPair(tickerName)) return null;

            // Valid ask, bid, volume
            if (ticker.symbol is null || ticker.ask is null || ticker.bid is null || ticker.quoteVolume is null) return null;

            string friendlySymbol = TickerLib.GetPureTicker(tickerName);
            return new ExchangeToken()
            {
                ExchangeId = exchange.id,
                DisplayName = friendlySymbol,
                Ask = ticker.ask ?? 0.0,
                Bid = ticker.bid ?? 0.0,
                AskVolume = ticker.askVolume ?? 0.0,
                BidVolume = ticker.bidVolume ?? 0.0,
                DayVolumeUSDT = ticker.quoteVolume ?? 0.0,
                Active = currency.active ?? false,
                Deposit = currency.deposit ?? false,
                Withdraw = currency.withdraw ?? false
            };
        }

        public ExchangeTokenNetwork CreateNetworkEntity(ExchangeToken owner, string code, in ccxt.Network network)
            => new()
            {
                ExchangeTokenId = owner.Id,
                Code = code,
                Deposit = network.deposit ?? false,
                Withdraw = network.withdraw ?? false,
                Active = network.active ?? false,
                Fee = network.fee
            };

        public async Task<ExchangeToken?> FindToken(string ownerId, string fullName, CancellationToken stoppingToken = default)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            IRepository<ExchangeToken, long> tokenRepo =
               scope.ServiceProvider.GetRequiredService<IRepository<ExchangeToken, long>>();
            return await tokenRepo.AsQueryable().FirstOrDefaultAsync(
               x => x.ExchangeId == ownerId && x.DisplayName == fullName,
               stoppingToken);
        }


        public async Task UpdateTokensNetworks(Exchange owner, CancellationToken stoppingToken = default)
        {
            Currencies currenciesContainer = await owner.FetchCurrencies();
            Tickers tickersContainer = await owner.FetchTickers();
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IRepository<ExchangeToken, long> tokenRepo =
                    scope.ServiceProvider.GetRequiredService<IRepository<ExchangeToken, long>>();

                //_logger.LogWarning($"{owner.id} -> {tickersContainer.tickers.Count}");
                foreach (KeyValuePair<string, Ticker> x in tickersContainer.tickers)
                {
                    if (string.IsNullOrEmpty(x.Value.symbol))
                        continue;
                    if (!TickerLib.IsUsdtPair(x.Value.symbol))
                        continue;
                    string friendlySymbol = TickerLib.GetPureTicker(x.Value.symbol);

                    if (!currenciesContainer.currencies.ContainsKey(friendlySymbol.ToUpper()))
                        continue;

                    KeyValuePair<string, Currency> currency = currenciesContainer.currencies.FirstOrDefault(a => a.Key == friendlySymbol.ToString());

                    ExchangeToken? tokenInDb = await FindToken(owner.id, friendlySymbol ?? "", stoppingToken);

                    if (tokenInDb is not null)
                    {

                        tokenInDb.Ask = x.Value.ask;
                        tokenInDb.Bid = x.Value.bid;
                        tokenInDb.AskVolume = x.Value.askVolume;
                        tokenInDb.BidVolume = x.Value.bidVolume;
                        tokenInDb.DayVolumeUSDT = x.Value.quoteVolume;
                        tokenInDb.Active = currency.Value.active ?? false;
                        tokenInDb.Deposit = currency.Value.deposit ?? false;
                        tokenInDb.Withdraw = currency.Value.withdraw ?? false;
                        tokenRepo.Update(tokenInDb, stoppingToken);
                    }
                    else
                    {
                        ExchangeToken? tokenEntity = CreateTokenEntity(owner, x.Value, currency.Value);
                        if (tokenEntity is not null)
                        {
                            await tokenRepo.Add(tokenEntity, stoppingToken);
                        }
                    }
                }
                await tokenRepo.SaveChangesAsync(stoppingToken);
            }

            // Update networsk
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IRepository<ExchangeTokenNetwork, long> networkRepo =
                    scope.ServiceProvider.GetRequiredService<IRepository<ExchangeTokenNetwork, long>>();
                foreach (KeyValuePair<string, Ticker> x in tickersContainer.tickers)
                {
                    if (string.IsNullOrEmpty(x.Value.symbol))
                        continue;

                    if (!TickerLib.IsUsdtPair(x.Value.symbol))
                        continue;
                    string friendlySymbol = TickerLib.GetPureTicker(x.Value.symbol);

                    if (!currenciesContainer.currencies.ContainsKey(friendlySymbol.ToUpper()))
                        continue;

                    KeyValuePair<string, Currency>? currency = currenciesContainer.currencies.FirstOrDefault(a => a.Key == friendlySymbol.ToString());
                    if (currency is null) continue;

                    ExchangeToken? tokenInDb = await FindToken(owner.id, friendlySymbol ?? "", stoppingToken);
                    if (tokenInDb is null) continue;

                    foreach (var net in currency.Value.Value.networks)
                    {
                        ExchangeTokenNetwork? networkInDb = networkRepo.AsQueryable().FirstOrDefault(x => x.ExchangeTokenId == tokenInDb.Id);
                        if (networkInDb is null)
                        {
                            ExchangeTokenNetwork newNetwork = CreateNetworkEntity(tokenInDb, net.Key, net.Value);
                            await networkRepo.Add(newNetwork, stoppingToken);
                        }
                        else
                        {
                            networkInDb.Active = net.Value.active ?? false;
                            networkInDb.Deposit = net.Value.deposit ?? false;
                            networkInDb.Withdraw = net.Value.withdraw ?? false;
                            networkInDb.Fee = net.Value.fee;
                        }
                    }
                }
                await networkRepo.SaveChangesAsync(stoppingToken);
            }
        }

        private static double _waitMax = 800.0;
        private static double _orderBookTasksMax = 10;
        private async Task ProcessOrderBook(Exchange owner, string sym, CancellationToken stoppingToken = default)
        {
            try
            {

                // Fetch order book by symbol. It will throw errors if failed
                OrderBook orderBook = await owner.FetchOrderBook(sym);

                // Must be valid orderbook
                if (orderBook.asks is null) throw new NullReferenceException(nameof(orderBook.asks));
                if (orderBook.bids is null) throw new NullReferenceException(nameof(orderBook.bids));

                //_logger.LogInformation($"[{owner.id}] {sym} book fetched ({orderBook.asks.First()[0]})");
                /*using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    IRepository<OrderBookItem, long> orderBookRepo =
                        scope.ServiceProvider.GetRequiredService<IRepository<OrderBookItem, long>>();
                    // Get current order book by symbol
                    IEnumerable<OrderBookItem> currentOrderBook = orderBookRepo
                    .AsQueryable()
                        .Where(x => x.ExchangeTokenId == token.Id)
                        .AsEnumerable();
                    orderBookRepo.DeleteRange(currentOrderBook, stoppingToken);
                }

                // Convert asks to items
                IEnumerable<OrderBookItem> asks = orderBook.asks.Select(x => new OrderBookItem()
                {
                    ExchangeTokenId = token.Id,
                    IsAsk = true,
                    Price = x[0],
                    Volume = x[1]
                });

                // Convert bids to items
                IEnumerable<OrderBookItem> bids = orderBook.bids.Select(x => new OrderBookItem()
                {
                    ExchangeTokenId = token.Id,
                    IsAsk = false,
                    Price = x[0],
                    Volume = x[1]
                });
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    IRepository<OrderBookItem, long> orderBookRepo =
                        scope.ServiceProvider.GetRequiredService<IRepository<OrderBookItem, long>>();
                    // Add asks and bids
                    await orderBookRepo.AddRange(asks.Concat(bids), stoppingToken);
                    // Save all changes
                    await orderBookRepo.SaveChangesAsync(stoppingToken);
                }*/
            }
            catch (Exception ex)
            {
                _logger.LogError($"{owner.id} ({sym} order book warn: {ex.Message}");
            }
            finally
            {

            }
        }
        public async Task UpdateTokensOrderBook(Exchange owner, CancellationToken stoppingToken = default)
        {
            Currencies currenciesContainer = await owner.FetchCurrencies();
            Tickers tickersContainer = await owner.FetchTickers();
            IList<ExchangeToken> tokens = [];
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IRepository<ExchangeToken, long> tokenRepo =
                    scope.ServiceProvider.GetRequiredService<IRepository<ExchangeToken, long>>();
                tokens = tokenRepo
                    .AsQueryable()
                    .Where(x => x.Active && x.ExchangeId == owner.id)
                    .ToList();
            }

            Stopwatch global = Stopwatch.StartNew();
            int n = tokens.Count;
            while(tokens.Count > 0)
            {
                IList<Task> tasks = [];
                Stopwatch stopwatch = Stopwatch.StartNew();
                for(int i = 0; i < _orderBookTasksMax; ++i)
                {
                    if (tokens.Count <= 0) break;
                    tasks.Add(ProcessOrderBook(owner, $"{tokens.First().DisplayName}/USDT", stoppingToken));
                    tokens.RemoveAt(0);
                }
                await Task.WhenAll(tasks);
                stopwatch.Stop();
                _logger.LogDebug($"[{owner.id}] {_orderBookTasksMax} items in {stopwatch.Elapsed.TotalSeconds:F3}s");
                if (stopwatch.Elapsed.TotalMilliseconds < _waitMax)
                {
                    _logger.LogDebug($"[{owner.id}] Waiting {_waitMax - stopwatch.Elapsed.TotalMilliseconds:F5}ms...");
                    await Task.Delay((int)(_waitMax - stopwatch.Elapsed.TotalMilliseconds), stoppingToken);
                }
            }
            _logger.LogInformation($"[{owner.id} -> {n} items in {global.Elapsed.TotalSeconds:F2}s");
        }
    }
}

