using ArbiDataLib.Models;

namespace ArbiReader.Services
{
    public interface IExchangeInfoService
    {
        public ExchangeInfo[] GetExchangeInfos();
        public ExchangeInfo? GetExchangeInfo(string exchangeId);
    }

    public class ExchangeInfoService(IConfiguration configuration) : IExchangeInfoService
    {
        private readonly IConfiguration _configuration = configuration;

        public ExchangeInfo? GetExchangeInfo(string exchangeId)
        {
            throw new NotImplementedException();
        }

        public ExchangeInfo[] GetExchangeInfos()
        {
            ExchangeInfo[]? res = _configuration.GetSection("Exchanges").Get<ExchangeInfo[]>();
            return res is null ? [] : res;
        }
    }
}
