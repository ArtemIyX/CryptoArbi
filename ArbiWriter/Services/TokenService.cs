using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiLib.Libs;
using ccxt;
using Microsoft.EntityFrameworkCore;

namespace ArbiWriter.Services
{
    public interface ITokenService
    {
        public Task UpdateTokens(Exchange owner, CancellationToken stoppingToken = default);
        public Task UpdateNetworks(Exchange owner, CancellationToken stoppingToken = default);
        public ExchangeToken? CreateTokenEntity(Exchange owner, in ccxt.Ticker ticker, in ccxt.Currency currency);
        public Task<ExchangeToken?> FindToken(string ownerId, string fullName, CancellationToken stoppingToken = default);
    }

    public class TokenService(IRepository<ExchangeToken, long> tokenRepository,
            IRepository<ExchangeTokenNetwork, long> networkRepository) : ITokenService
    {
        private readonly IRepository<ExchangeToken, long> _tokenRepo = tokenRepository;
        private readonly IRepository<ExchangeTokenNetwork, long> _networkRepo = networkRepository;

        public ExchangeToken? CreateTokenEntity(Exchange exchange, in ccxt.Ticker ticker, in ccxt.Currency currency)
        {
            string tickerName = TickerLib.RemoveSemiColon(ticker.symbol ?? "");

            // Must be X/USDT
            if (!TickerLib.IsUsdtPair(tickerName)) return null;

            // Valid ask, bid, volume
            if (ticker.symbol is null || ticker.ask is null || ticker.bid is null || ticker.quoteVolume is null
                || ticker.askVolume is null || ticker.bidVolume is null) return null;

            string friendlySymbol = TickerLib.GetPureTicker(tickerName);
            return new ExchangeToken()
            {
                FullSymbolName = ticker.symbol,
                ExchangeId = exchange.id,
                DisplayName = friendlySymbol,
                Ask = ticker.ask,
                Bid = ticker.bid,
                AskVolume = ticker.askVolume,
                BidVolume = ticker.bidVolume,
                DayVolumeUSDT = ticker.quoteVolume,
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
                Active = network.active ?? false
            };

        public async Task<ExchangeToken?> FindToken(string ownerId, string fullName, CancellationToken stoppingToken = default)
            => await _tokenRepo.GetDbSet().FirstOrDefaultAsync(
                x => x.ExchangeId == ownerId && x.FullSymbolName == fullName,
                stoppingToken);

        public async Task UpdateNetworks(Exchange owner, CancellationToken stoppingToken = default)
        {
            Currencies currenciesContainer = await owner.FetchCurrencies();
            Tickers tickersContainer = await owner.FetchTickers();
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

                ExchangeToken? tokenInDb = await FindToken(owner.id, x.Value.symbol ?? "", stoppingToken);
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
                    }
                }
                await Task.WhenAll(addTasks);
            }
            await _networkRepo.SaveChangesAsync();
        }


        public async Task UpdateTokens(Exchange owner, CancellationToken stoppingToken = default)
        {
            Currencies currenciesContainer = await owner.FetchCurrencies();
            Tickers tickersContainer = await owner.FetchTickers();
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

                ExchangeToken? tokenInDb = await FindToken(owner.id, x.Value.symbol ?? "", stoppingToken);

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
        }
    }
}
