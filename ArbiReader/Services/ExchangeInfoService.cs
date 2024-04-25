using ArbiDataLib.Models;
using Nethereum.Util;

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
        private readonly string InfoSection = "ExUrls";
        public ExchangeUrlInfo? GetExchangeInfo(string exchangeId)
        {
            ExchangeUrlInfo[]? res = _configuration.GetSection(InfoSection).Get<ExchangeUrlInfo[]>();
            if (res is null) return null;
            return res.FirstOrDefault(x => x.ExchangeId == exchangeId);
        }

        public ExchangeUrlInfo[] GetExchangeInfos()
        {
            ExchangeUrlInfo[]? res = _configuration.GetSection(InfoSection).Get<ExchangeUrlInfo[]>();
            return res is null ? [] : res;
        }
    }
}
