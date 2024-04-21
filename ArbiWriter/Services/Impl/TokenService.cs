using ArbiLib.Libs;
using ArbiDataLib.Models;
using ArbiWriter.Services.Interfaces;
using ccxt;
using Microsoft.EntityFrameworkCore;
using Nethereum.Util;
using System.Data;
using ArbiDataLib.Data.Repo;

namespace ArbiWriter.Services.Impl
{
    public class TokenService(IRepository<ExchangeToken, long> tokenRepository) : ITokenService
    {
        private readonly IRepository<ExchangeToken, long> _tokenRepo = tokenRepository;

        public ExchangeToken? CreateTokenEntity(Exchange exchange, in Ticker ticker)
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
                DayVolumeUSDT = ticker.quoteVolume
            };
        }

        public async Task<ExchangeToken?> FindToken(string ownerId, string fullName, CancellationToken stoppingToken = default)
            => await _tokenRepo.GetDbSet().FirstOrDefaultAsync(
                x => x.ExchangeId == ownerId && x.FullSymbolName == fullName,
                stoppingToken);


        public async Task UpdateTokens(ccxt.Exchange owner, CancellationToken stoppingToken = default)
        {
            Tickers container = await owner.FetchTickers();
            foreach (KeyValuePair<string, Ticker> x in container.tickers)
            {
                ExchangeToken? tokenInDb = await FindToken(owner.id, x.Value.symbol ?? "", stoppingToken);
                
                if (tokenInDb is not null)
                {
                    tokenInDb.Ask = x.Value.ask;
                    tokenInDb.Bid = x.Value.bid;
                    tokenInDb.AskVolume = x.Value.askVolume;
                    tokenInDb.BidVolume = x.Value.bidVolume;
                    tokenInDb.DayVolumeUSDT = x.Value.quoteVolume;
                    _tokenRepo.Update(tokenInDb, stoppingToken);
                }
                else
                {
                    if(x.Value.symbol is not null && TickerLib.IsUsdtPair(x.Value.symbol))
                    {
                        ExchangeToken? tokenEntity = CreateTokenEntity(owner, x.Value);
                        if (tokenEntity is not null)
                        {
                            await _tokenRepo.Add(tokenEntity, stoppingToken);
                        }
                    }
                   
                }
            }
            await _tokenRepo.SaveChangesAsync(stoppingToken);
        }
    }
}
