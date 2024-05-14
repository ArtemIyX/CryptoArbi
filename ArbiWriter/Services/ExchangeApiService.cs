using ArbiDataLib.Data;
using ccxt;

namespace ArbiWriter.Services
{
    public interface IExchangeApiService
    {
        public ExchangeApiInfo[] GetExchangeApi();
        public ExchangeApiInfo? GetExchangeApi(string exchangeId);
        public ccxt.Exchange Fill(ccxt.Exchange ex);
    }

    public class ExchangeApiService(IConfiguration configuration) : IExchangeApiService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly string ApiSection = "ExApi";

        public Exchange Fill(Exchange ex)
        {
            ExchangeApiInfo? info = GetExchangeApi(ex.id);
            if (info is null) return ex;
            ex.apiKey = info.Api;
            ex.secret = info.Secret;
            return ex;
        }

        public ExchangeApiInfo[] GetExchangeApi()
        {
            ExchangeApiInfo[]? res = _configuration.GetSection(ApiSection).Get<ExchangeApiInfo[]>();
            return res is null ? [] : res;
        }

        public ExchangeApiInfo? GetExchangeApi(string exchangeId)
        {
            ExchangeApiInfo[]? res = _configuration.GetSection(ApiSection).Get<ExchangeApiInfo[]>();
            if (res is null) return null;
            return res.FirstOrDefault(x => x.ExchangeId == exchangeId);
        }
    }
}
