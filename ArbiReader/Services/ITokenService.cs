using ArbiDataLib.Data;
using ArbiDataLib.Models;

namespace ArbiReader.Services
{
    public interface ITokenService
    {
        public Task<IList<ExchangeTokenResponse>> GetBySymbol(string symbol);
        public Task<ExchangeTokenResponse?> GetById(long tokenId);
        //public Task<ExchangeEntityResponse?> GetTokenExchange(long tokenId);
        public Task<List<ArbiItem>> GetArbi(ArbiFilter filter);
    }
}
