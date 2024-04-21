using ArbiDataLib.Models;
using ArbiReader.Data;
using ArbiReader.Data.Responses;

namespace ArbiReader.Services
{
    public interface ITokenService
    {
        public Task<IList<ExchangeTokenResponse>> GetBySymbol(string symbol);
        public Task<ExchangeTokenResponse?> GetById(long tokenId);
        //public Task<ExchangeEntityResponse?> GetTokenExchange(long tokenId);
        public Task<List<ArbiItem>> GetArbi(ArbiFilter InFilter);
    }
}
