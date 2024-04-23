using ArbiDataLib.Models;
using ArbiReader.Data;
using ccxt;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace ArbiBlazor.Services
{
    public interface IExchangeService
    {
        public Task<List<ExchangeEntityResponse>> GetWorkingExchanges();
        public Task<ExchangeEntityResponse?> GetExchange(string exchangeId);
        public Task<string> Test();
    }

    public class ExchangeService(HttpClient httpClient,
        IAppSettingsService appSettingsService) : IExchangeService
    {
        private readonly HttpClient _http = httpClient;
        private readonly IAppSettingsService _appSettings = appSettingsService;

        public async Task<ExchangeEntityResponse?> GetExchange(string exchangeId)
        {

            return null;
        }

        public async Task<List<ExchangeEntityResponse>> GetWorkingExchanges()
        {

            return new List<ExchangeEntityResponse>();
        }

        public async Task<string> Test()
        {
            try
            {
                string url = $"api/exchanges";
                var str = await _http.GetStringAsync(url);
                return str;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
