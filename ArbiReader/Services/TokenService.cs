
using ArbiDataLib.Data;
using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ArbiReader.Services
{
    public interface ITokenService
    {
        public Task<IList<ExchangeTokenResponse>> GetTokens(string symbol);
        public Task<ExchangeTokenResponse?> GetToken(long tokenId);
        public Task<IList<ArbiItem>> GetArbi(ArbiFilter filter);
        public Task<IList<ArbiItem>> GetArbiBySymbol(string symbol);
    }
    public class TokenService(IRepository<ExchangeToken, long> tokenRepository) : ITokenService
    {
        private readonly IRepository<ExchangeToken, long> _tokenRepo = tokenRepository;

        public async Task<ExchangeTokenResponse?> GetToken(long tokenId) =>
            (await _tokenRepo
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == tokenId))?
            .ToResponse();

        public async Task<IList<ExchangeTokenResponse>> GetTokens(string symbol) =>
            await _tokenRepo.AsQueryable()
            .Where(x => x.DisplayName == symbol)
            .Select(x => x.ToResponse())
            .ToListAsync();

        public async Task<IList<ArbiItem>> GetArbi(ArbiFilter filter)
        {
            return await Task.Run(() =>
            {
                string[] buy_ban = filter.MakerForbiddenBuy();
                string[] sell_ban = filter.MakerForbiddenSell();
                IQueryable<ArbiItem> rankedTokens = from t1 in _tokenRepo.AsQueryable()
                                                    join t2 in _tokenRepo.AsQueryable() on t1.DisplayName equals t2.DisplayName
                                                    let diff = ((t2.Bid - t1.Ask) / t1.Ask) * 100
                                                    where t1.ExchangeId != t2.ExchangeId && // different exchanges
                                                          !buy_ban.Contains(t1.ExchangeId) &&
                                                          !sell_ban.Contains(t2.ExchangeId) &&
                                                          t1.Exchange!.Working &&
                                                          t2.Exchange!.Working &&
                                                          t1.Ask < t2.Bid &&              // spread
                                                          t1.Ask > filter.MinPrice &&      // ask filter
                                                          t2.Bid > filter.MinPrice &&      // bid filter
                                                          t1.AskVolume * t1.Ask > filter.MinAskVolumeUSDT && // ask volume filter
                                                          t2.BidVolume * t2.Bid > filter.MinBidVolumeUSDT && // bid volume filter
                                                          t1.DayVolumeUSDT > filter.MinAskDayVolumeUSDT && // dayvolume ask filter
                                                          t2.DayVolumeUSDT > filter.MinBidDayVolumeUSDT &&
                                                          diff > filter.MinPercent &&
                                                          diff < filter.MaxPercent
                                                    orderby diff descending
                                                    select new ArbiItem()
                                                    {
                                                        DisplayName = t1.DisplayName,
                                                        ExchangeId1 = t1.ExchangeId,
                                                        ExchangeId2 = t2.ExchangeId,
                                                        AskId = t1.Id,
                                                        Ask = t1.Ask ?? 0.0,
                                                        AskVolume = t1.AskVolume ?? 0.0,
                                                        AskVolumeUSDT = t1.AskVolume * t1.Ask ?? 0.0,
                                                        AskDayVolumeUSDT = t1.DayVolumeUSDT?? 0.0,
                                                        BidId = t2.Id,
                                                        Bid = t2.Bid ?? 0.0,
                                                        BidVolume = t2.BidVolume ?? 0.0,
                                                        BidVolumeUSDT = t2.BidVolume * t2.Bid ?? 0.0,
                                                        BidDayVolumeUSDT = t2.DayVolumeUSDT ?? 0.0,
                                                        FullSymbolName = t1.FullSymbolName,
                                                        PriceDifferencePercentage = diff ?? 0.0,
                                                        Updated = t1.Updated,
                                                    };


                var numerable = rankedTokens.AsEnumerable();
                var distinct = numerable.DistinctBy(x => x.DisplayName);
                var taken = distinct.Take(filter.Amount);
                return taken.ToList();
            });
        }

        public async Task<IList<ArbiItem>> GetArbiBySymbol(string symbol)
        {
            IQueryable<ArbiItem> rankedTokens = from t1 in _tokenRepo.AsQueryable()
                                                join t2 in _tokenRepo.AsQueryable() on t1.DisplayName equals t2.DisplayName
                                                let diff = ((t2.Bid - t1.Ask) / t1.Ask) * 100
                                                where t1.ExchangeId != t2.ExchangeId && // different exchanges
                                                      t1.Exchange!.Working &&
                                                      t2.Exchange!.Working &&
                                                      t1.Ask < t2.Bid && // spread 
                                                      t1.DisplayName == symbol &&
                                                      t2.DisplayName == symbol
                                                orderby diff descending
                                                select new ArbiItem()
                                                {
                                                    DisplayName = t1.DisplayName,
                                                    ExchangeId1 = t1.ExchangeId,
                                                    ExchangeId2 = t2.ExchangeId,
                                                    AskId = t1.Id,
                                                    Ask = t1.Ask ?? 0.0,
                                                    AskVolume = t1.AskVolume ?? 0.0,
                                                    AskVolumeUSDT = t1.AskVolume * t1.Ask ?? 0.0,
                                                    AskDayVolumeUSDT = t1.DayVolumeUSDT ?? 0.0,
                                                    BidId = t2.Id,
                                                    Bid = t2.Bid ?? 0.0,
                                                    BidVolume = t2.BidVolume ?? 0.0,
                                                    BidVolumeUSDT = t2.BidVolume * t2.Bid ?? 0.0,
                                                    BidDayVolumeUSDT = t2.DayVolumeUSDT ?? 0.0,
                                                    FullSymbolName = t1.FullSymbolName,
                                                    PriceDifferencePercentage = diff ?? 0.0,
                                                    Updated = t1.Updated
                                                };

            return await rankedTokens.ToListAsync();

        }
    }
}
