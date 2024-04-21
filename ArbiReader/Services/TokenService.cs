
using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiReader.Data;
using ArbiReader.Data.Responses;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArbiReader.Services
{
    public class TokenService(IRepository<ExchangeToken, long> tokenRepository) : ITokenService
    {
        private readonly IRepository<ExchangeToken, long> _tokenRepo = tokenRepository;

        public async Task<ExchangeTokenResponse?> GetById(long tokenId) =>
            (await _tokenRepo
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == tokenId))?
            .ToResponse();

        public async Task<IList<ExchangeTokenResponse>> GetBySymbol(string symbol) =>
            await _tokenRepo.AsQueryable()
            .Where(x => x.DisplayName == symbol)
            .Select(x => x.ToResponse())
            .ToListAsync();

/*        public async Task<ExchangeEntityResponse?> GetTokenExchange(long tokenId) =>
            (await _tokenRepo
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == tokenId))
            ?.Exchange?.ToReposnse();*/

        public async Task<List<ArbiItem>> GetArbi(ArbiFilter InFilter)
        {
            IQueryable<ArbiItem> rankedTokens = from t1 in _tokenRepo.AsQueryable()
                         join t2 in _tokenRepo.AsQueryable() on t1.DisplayName equals t2.DisplayName
                         let diff = ((t2.Bid - t1.Ask) / t1.Ask) * 100
                         where t1.ExchangeId != t2.ExchangeId && // different exchanges
                               t1.Ask < t2.Bid &&               // spread
                               t1.Ask > InFilter.MinAsk &&      // ask filter
                               t2.Bid > InFilter.MinBid &&      // bid filter
                               t1.AskVolume * t1.Ask > InFilter.MinAskVolumeUsdt && // ask volume filter
                               t2.BidVolume * t2.Bid > InFilter.MinBidVolumeUsdt && // bid volume filter
                               t1.DayVolumeUSDT > InFilter.MinDayVolumeUsdt && // dayvolume ask filter
                               t2.DayVolumeUSDT > InFilter.MinDayVolumeUsdt &&
                               diff > InFilter.MinPercent &&
                               diff < InFilter.MaxPercent
                         orderby diff descending
                         select new ArbiItem()
                         {
                             DisplayName = t1.DisplayName,
                             ExchangeId1 = t1.ExchangeId,
                             ExchangeId2 = t2.ExchangeId,
                             AskId = t1.Id,
                             Ask = t1.Ask ?? 0.0,
                             AskVolume = t1.AskVolume ?? 0.0,
                             AskVolumeUsdt = t1.AskVolume * t1.Ask ?? 0.0,
                             BidId = t2.Id,
                             Bid = t2.Bid ?? 0.0,
                             BidVolume = t2.BidVolume ?? 0.0,
                             BidVolumeUsdt = t2.BidVolume * t2.Bid ?? 0.0,
                             FullSymbolName = t1.FullSymbolName,
                             PriceDifferencePercentage = diff ?? 0.0,
                         };

            var taken = rankedTokens.Take(InFilter.Amount);

            return await taken.ToListAsync();
        }
    }
}
