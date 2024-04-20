using ArbiWriter.Models;

namespace ArbiWriter.Services.Interfaces
{
    public interface ITokenService
    {
        Task UpdateTokens(ccxt.Exchange owner, CancellationToken stoppingToken = default);
        ExchangeToken? CreateTokenEntity(ccxt.Exchange owner, in ccxt.Ticker ticker);
        Task<ExchangeToken?> FindToken(string ownerId, string fullName, CancellationToken stoppingToken = default);
    }
}
