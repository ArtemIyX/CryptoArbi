using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiLib.Libs;
using ccxt;
using Microsoft.EntityFrameworkCore;

namespace ArbiWriter.Services
{
    public interface ITokenService
    {
        public Task UpdateTokensNetworks(Exchange owner, CancellationToken stoppingToken = default);
        public Task UpdateTokensOrderBook(Exchange owner, CancellationToken stoppingToken = default);
        public ExchangeToken? CreateTokenEntity(Exchange owner, in ccxt.Ticker ticker, in ccxt.Currency currency);
        public Task<ExchangeToken?> FindToken(string ownerId, string fullName, CancellationToken stoppingToken = default);
    }

    public class TokenService(IRepository<ExchangeToken, long> tokenRepository,
            IRepository<ExchangeTokenNetwork, long> networkRepository,
            IRepository<OrderBookItem, long> orderBookRepository,
            ILogger<TokenService> logger) : ITokenService
    {
        private readonly IRepository<ExchangeToken, long> _tokenRepo = tokenRepository;
        private readonly IRepository<OrderBookItem, long> _orderBookRepo = orderBookRepository;
        private readonly IRepository<ExchangeTokenNetwork, long> _networkRepo = networkRepository;
        private readonly ILogger<TokenService> _logger = logger;


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
            => await _tokenRepo.GetDbSet().FirstOrDefaultAsync(
                x => x.ExchangeId == ownerId && x.DisplayName == fullName,
                stoppingToken);

        public async Task UpdateTokensNetworks(Exchange owner, CancellationToken stoppingToken = default)
        {
            Currencies currenciesContainer = await owner.FetchCurrencies();
            Tickers tickersContainer = await owner.FetchTickers();

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
                    _tokenRepo.Update(tokenInDb, stoppingToken);
                }
                else
                {
                    ExchangeToken? tokenEntity = CreateTokenEntity(owner, x.Value, currency.Value);
                    if (tokenEntity is not null)
                    {
                        await _tokenRepo.Add(tokenEntity, stoppingToken);
                    }
                }
            }
            await _tokenRepo.SaveChangesAsync(stoppingToken);


            // Update networsk

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

                IList<Task> addTasks = [];
                foreach (var net in currency.Value.Value.networks)
                {
                    ExchangeTokenNetwork? networkInDb = _networkRepo.AsQueryable().FirstOrDefault(x => x.ExchangeTokenId == tokenInDb.Id);
                    if (networkInDb is null)
                    {
                        ExchangeTokenNetwork newNetwork = CreateNetworkEntity(tokenInDb, net.Key, net.Value);
                        addTasks.Add(_networkRepo.Add(newNetwork, stoppingToken));
                    }
                    else
                    {
                        networkInDb.Active = net.Value.active ?? false;
                        networkInDb.Deposit = net.Value.deposit ?? false;
                        networkInDb.Withdraw = net.Value.withdraw ?? false;
                        networkInDb.Fee = net.Value.fee;
                    }
                }
                await Task.WhenAll(addTasks);
            }
            await _networkRepo.SaveChangesAsync();
        }

        public async Task UpdateTokensOrderBook(Exchange owner, CancellationToken stoppingToken = default)
        {
            var tokens = await owner.FetchTickers();
            //_logger.LogInformation($"{owner.id} -> {tokens.Count}");
            int i = 0;
            foreach (var item in tokens.tickers)
            {
                try
                {
                    await owner.FetchOrderBook(item.Value.symbol);
                    _logger.LogInformation($"[{owner.id}]Book: {item.Value.symbol} -> {i}|{tokens.tickers.Count}");
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    ++i;
                }
            }
            //OrderBook? res = 
        }
    }
}
