using ArbiDataLib.Models;

namespace ArbiReader.Services
{
    public interface IExchangeInfoService
    {
        public ExchangeUrlInfo[] GetExchangeInfos();
        public ExchangeUrlInfo? GetExchangeInfo(string exchangeId);
    }

    public class ExchangeInfoService(IConfiguration configuration) : IExchangeInfoService
    {
        private readonly IConfiguration _configuration = configuration;

        public ExchangeUrlInfo? GetExchangeInfo(string exchangeId)
        {
            throw new NotImplementedException();
        }

        public ExchangeUrlInfo[] GetExchangeInfos()
        {
            ExchangeUrlInfo[]? res = _configuration.GetSection("Exchanges").Get<ExchangeUrlInfo[]>();
            return res is null ? [] : res;
        }
    }
}
